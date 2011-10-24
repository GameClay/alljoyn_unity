/**
 * @file
 *
 * This file implements the ProxyBusObject class.
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

#include <qcc/platform.h>

#include <assert.h>
#include <alljoyn/ProxyBusObject.h>

#include <Status.h>

#define QCC_MODULE "ALLJOYN"

struct _alljoyn_proxybusobject_handle {
    /* Empty by design, this is just to allow the type restrictions to save coders from themselves */
};

alljoyn_proxybusobject alljoyn_proxybusobject_create(alljoyn_busattachment bus, const char* service,
                                                     const char* path, alljoyn_sessionid sessionId)
{
    ajn::ProxyBusObject* ret = new ajn::ProxyBusObject(*((ajn::BusAttachment*)bus), service, path, sessionId);
    return (alljoyn_proxybusobject)ret;
}

void alljoyn_proxybusobject_destroy(alljoyn_proxybusobject bus)
{
    assert(bus != NULL && "NULL parameter passed to alljoyn_proxybusobject_destroy.");
    delete (ajn::ProxyBusObject*)bus;
}

QStatus alljoyn_proxybusobject_addinterface(alljoyn_proxybusobject bus, const alljoyn_interfacedescription iface)
{
    return ((ajn::ProxyBusObject*)bus)->AddInterface(*((const ajn::InterfaceDescription*)iface));
}

QStatus alljoyn_proxybusobject_methodcall_synch(alljoyn_proxybusobject obj,
                                                const char* ifaceName,
                                                const char* methodName,
                                                alljoyn_msgargs args,
                                                size_t numArgs,
                                                alljoyn_message replyMsg,
                                                uint32_t timeout,
                                                uint8_t flags)
{
    ajn::Message* reply = (ajn::Message*)&(*replyMsg);
    return ((ajn::ProxyBusObject*)obj)->MethodCall(ifaceName, methodName, (const ajn::MsgArg*)args,
                                                   numArgs, *reply, timeout, flags);
}
