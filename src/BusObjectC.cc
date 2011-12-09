/**
 * @file
 *
 * This file implements a BusObject subclass for use by the C API
 */

/******************************************************************************
 * Copyright 2009-2011, Qualcomm Innovation Center, Inc.
 *
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 ******************************************************************************/

#include <alljoyn/BusObject.h>
#include <alljoyn_unity/BusObject.h>
#include <map>
#include "DeferredCallback.h"

using namespace qcc;
using namespace std;

namespace ajn {

class BusObjectC : public BusObject {
  public:
    BusObjectC(alljoyn_busattachment bus, const char* path, QC_BOOL isPlaceholder, \
               const alljoyn_busobject_callbacks* callbacks_in, const void* context_in) :
        BusObject(*((BusAttachment*)bus), path, isPlaceholder == QC_TRUE ? true : false)
    {
        context = context_in;
        memcpy(&callbacks, callbacks_in, sizeof(alljoyn_busobject_callbacks));
    }

    QStatus MethodReplyC(alljoyn_message msg, const alljoyn_msgargs args, size_t numArgs)
    {
        return MethodReply(*((Message*)msg), (const MsgArg*)args, numArgs);
    }

    QStatus MethodReplyC(alljoyn_message msg, const char* error, const char* errorMessage)
    {
        return MethodReply(*((Message*)msg), error, errorMessage);
    }

    QStatus MethodReplyC(alljoyn_message msg, QStatus status)
    {
        return MethodReply(*((Message*)msg), status);
    }
#if 0
    QStatus SignalC(const char* destination,
                    alljoyn_sessionid sessionId,
                    const InterfaceDescription::Member& signal,
                    const alljoyn_msgargs args,
                    size_t numArgs,
                    uint16_t timeToLive,
                    uint8_t flags)
    {

    }
#endif
    QStatus AddInterfaceC(const alljoyn_interfacedescription iface)
    {
        return AddInterface(*(const InterfaceDescription*)iface);
    }

    QStatus AddMethodHandlersC(const alljoyn_busobject_methodentry* entries, size_t numEntries)
    {
        MethodEntry* proper_entries = new MethodEntry[numEntries];

        for (size_t i = 0; i < numEntries; i++) {
            proper_entries[i].member = (const ajn::InterfaceDescription::Member*)entries[i].member->internal_member;
            callbackMap.insert(pair<const ajn::InterfaceDescription::Member*, alljoyn_messagereceiver_methodhandler_ptr>(
                                   (const ajn::InterfaceDescription::Member*)entries[i].member->internal_member,
                                   entries[i].method_handler));
            proper_entries[i].handler = static_cast<MessageReceiver::MethodHandler>(&BusObjectC::MethodHandlerRemap);
        }
        QStatus ret = AddMethodHandlers(proper_entries, numEntries);
        delete [] proper_entries;
        return ret;
    }


  protected:
    virtual QStatus Get(const char* ifcName, const char* propName, MsgArg& val)
    {
        QStatus ret = ER_BUS_NO_SUCH_PROPERTY;
        if (callbacks.property_get != NULL) {
            DeferredCallback_4<QStatus, const void*, const char*, const char*, alljoyn_msgargs>* dcb =
                new DeferredCallback_4<QStatus, const void*, const char*, const char*, alljoyn_msgargs>(callbacks.property_get, context, ifcName, propName, (alljoyn_msgargs)(&val));
            ret = DEFERRED_CALLBACK_EXECUTE(dcb);
        }
        return ret;
    }

    virtual QStatus Set(const char* ifcName, const char* propName, MsgArg& val)
    {
        QStatus ret = ER_BUS_NO_SUCH_PROPERTY;
        if (callbacks.property_set != NULL) {
            DeferredCallback_4<QStatus, const void*, const char*, const char*, alljoyn_msgargs>* dcb =
                new DeferredCallback_4<QStatus, const void*, const char*, const char*, alljoyn_msgargs>(callbacks.property_set, context, ifcName, propName, (alljoyn_msgargs)(&val));
            ret = DEFERRED_CALLBACK_EXECUTE(dcb);
        }
        return ret;
    }

    // TODO: Need to do GenerateIntrospection?

    virtual void ObjectRegistered(void)
    {
        if (callbacks.object_registered != NULL) {
            DeferredCallback_1<void, const void*>* dcb =
                new DeferredCallback_1<void, const void*>(callbacks.object_registered, context);
            DEFERRED_CALLBACK_EXECUTE(dcb);
        }
    }

    virtual void ObjectUnregistered(void)
    {
        // Call parent as per docs
        BusObject::ObjectUnregistered();

        if (callbacks.object_unregistered != NULL) {
            DeferredCallback_1<void, const void*>* dcb =
                new DeferredCallback_1<void, const void*>(callbacks.object_unregistered, context);
            DEFERRED_CALLBACK_EXECUTE(dcb);
        }
    }

    // TODO: Need to do GetProp, SetProp, GetAllProps, Introspect?

  private:
    void MethodHandlerRemap(const InterfaceDescription::Member* member, Message& message)
    {
        /* Populate a C member description for use in the callback */
        alljoyn_interfacedescription_member c_member;

        c_member.iface = (alljoyn_interfacedescription)member->iface;
        c_member.memberType = (alljoyn_messagetype)member->memberType;
        c_member.name = member->name.c_str();
        c_member.signature = member->signature.c_str();
        c_member.returnSignature = member->returnSignature.c_str();
        c_member.argNames = member->argNames.c_str();
        c_member.annotation = member->annotation;
        c_member.internal_member = member;

        /* Look up the C callback via map and invoke */
        alljoyn_messagereceiver_methodhandler_ptr remappedHandler = callbackMap[member];
        remappedHandler((alljoyn_busobject) this, &c_member, (alljoyn_message)(&message));
    }

    map<const ajn::InterfaceDescription::Member*, alljoyn_messagereceiver_methodhandler_ptr> callbackMap;
    alljoyn_busobject_callbacks callbacks;
    const void* context;
};
}

struct _alljoyn_busobject_handle {
    /* Empty by design, this is just to allow the type restrictions to save coders from themselves */
};

alljoyn_busobject alljoyn_busobject_create(alljoyn_busattachment bus, const char* path, QC_BOOL isPlaceholder,
                                           const alljoyn_busobject_callbacks* callbacks_in, const void* context_in)
{
    return (alljoyn_busobject) new ajn::BusObjectC(bus, path, isPlaceholder, callbacks_in, context_in);
}

void alljoyn_busobject_destroy(alljoyn_busobject bus)
{
    delete (ajn::BusObjectC*)bus;
}

const char* alljoyn_busobject_getpath(alljoyn_busobject bus)
{
    return ((ajn::BusObjectC*)bus)->GetPath();
}

size_t alljoyn_busobject_getname(alljoyn_busobject bus, char* buffer, size_t bufferSz)
{
    qcc::String name = ((ajn::BusObjectC*)bus)->GetName();
    if (buffer != NULL && bufferSz > 0) {
        buffer[bufferSz - 1] = '\0';
        strncpy(buffer, name.c_str(), bufferSz - 1);
    }
    return name.length();
}

QStatus alljoyn_busobject_addinterface(alljoyn_busobject bus, const alljoyn_interfacedescription iface)
{
    return ((ajn::BusObjectC*)bus)->AddInterfaceC(iface);
}

QStatus alljoyn_busobject_addmethodhandlers(alljoyn_busobject bus, const alljoyn_busobject_methodentry* entries, size_t numEntries)
{
    return ((ajn::BusObjectC*)bus)->AddMethodHandlersC(entries, numEntries);
}

QStatus alljoyn_busobject_methodreply_args(alljoyn_busobject bus, alljoyn_message msg,
                                           const alljoyn_msgargs args, size_t numArgs)
{
    return ((ajn::BusObjectC*)bus)->MethodReplyC(msg, args, numArgs);
}

QStatus alljoyn_busobject_methodreply_err(alljoyn_busobject bus, alljoyn_message msg,
                                          const char* error, const char* errorMessage)
{
    return ((ajn::BusObjectC*)bus)->MethodReplyC(msg, error, errorMessage);
}

QStatus alljoyn_busobject_methodreply_status(alljoyn_busobject bus, alljoyn_message msg, QStatus status)
{
    return ((ajn::BusObjectC*)bus)->MethodReplyC(msg, status);
}

