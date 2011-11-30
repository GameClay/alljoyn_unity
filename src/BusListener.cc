/**
 * @file
 * This implements the C accessable version of the BusListener class using
 * function pointers, and a pass-through implementation of BusListener.
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

#include <alljoyn/BusListener.h>
#include <alljoyn_unity/BusListener.h>
#include <string.h>
#include <assert.h>
#include "DeferredCallback.h"

namespace ajn {

/**
 * Abstract base class implemented by AllJoyn users and called by AllJoyn to inform
 * users of bus related events.
 */
class BusListenerCallbackC : BusListener {
  public:
    BusListenerCallbackC(const alljoyn_buslistener_callbacks* in_callbacks, const void* in_context)
    {
        memcpy(&callbacks, in_callbacks, sizeof(alljoyn_buslistener_callbacks));
        context = in_context;
    }

    void ListenerRegistered(BusAttachment* bus)
    {
        if (callbacks.listener_registered != NULL) {
            DeferredCallback_2<void, const void*, alljoyn_busattachment>* dcb =
                new DeferredCallback_2<void, const void*, alljoyn_busattachment>(callbacks.listener_registered, context, (alljoyn_busattachment)bus);
            dcb->Execute();
        }
    }

    void ListenerUnregistered()
    {
        if (callbacks.listener_unregistered != NULL) {
            DeferredCallback_1<void, const void*>* dcb =
                new DeferredCallback_1<void, const void*>(callbacks.listener_unregistered, context);
            dcb->Execute();
        }
    }

    void FoundAdvertisedName(const char* name, TransportMask transport, const char* namePrefix)
    {
        if (callbacks.found_advertised_name != NULL) {
            DeferredCallback_4<void, const void*, const char*, TransportMask, const char*>* dcb =
                new DeferredCallback_4<void, const void*, const char*, TransportMask, const char*>(callbacks.found_advertised_name, context, name, transport, namePrefix);
            dcb->Execute();
        }
    }

    void LostAdvertisedName(const char* name, TransportMask transport, const char* namePrefix)
    {
        if (callbacks.lost_advertised_name != NULL) {
            DeferredCallback_4<void, const void*, const char*, TransportMask, const char*>* dcb =
                new DeferredCallback_4<void, const void*, const char*, TransportMask, const char*>(callbacks.lost_advertised_name, context, name, transport, namePrefix);
            dcb->Execute();
        }
    }

    void NameOwnerChanged(const char* busName, const char* previousOwner, const char* newOwner)
    {
        if (callbacks.name_owner_changed != NULL) {
            DeferredCallback_4<void, const void*, const char*, const char*, const char*>* dcb =
                new DeferredCallback_4<void, const void*, const char*, const char*, const char*>(callbacks.name_owner_changed, context, busName, previousOwner, newOwner);
            dcb->Execute();
        }
    }

    void BusStopping()
    {
        if (callbacks.bus_stopping != NULL) {
            DeferredCallback_1<void, const void*>* dcb =
                new DeferredCallback_1<void, const void*>(callbacks.bus_stopping, context);
            dcb->Execute();
        }
    }

    void BusDisconnected()
    {
        if (callbacks.bus_disconnected != NULL) {
            DeferredCallback_1<void, const void*>* dcb =
                new DeferredCallback_1<void, const void*>(callbacks.bus_disconnected, context);
            dcb->Execute();
        }
    }

  protected:
    alljoyn_buslistener_callbacks callbacks;
    const void* context;
};

}

struct _alljoyn_buslistener_handle {
    /* Empty by design, this is just to allow the type restrictions to save coders from themselves */
};

alljoyn_buslistener alljoyn_buslistener_create(const alljoyn_buslistener_callbacks* callbacks, const void* context)
{
    return (alljoyn_buslistener) new ajn::BusListenerCallbackC(callbacks, context);
}

void alljoyn_buslistener_destroy(alljoyn_buslistener listener)
{
    assert(listener != NULL && "listener parameter must not be NULL");
    delete (ajn::BusListenerCallbackC*)listener;
}

