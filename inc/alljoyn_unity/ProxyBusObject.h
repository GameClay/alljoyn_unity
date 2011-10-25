#ifndef _ALLJOYN_UNITY_REMOTEBUSOBJECT_H
#define _ALLJOYN_UNITY_REMOTEBUSOBJECT_H
/**
 * @file
 * This file defines the class ProxyBusObject.
 * The ProxyBusObject represents a single object registered  registered on the bus.
 * ProxyBusObjects are used to make method calls on these remotely located DBus objects.
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
#include <alljoyn_unity/AjAPI.h>
#include <alljoyn_unity/InterfaceDescription.h>
#include <alljoyn_unity/MessageReceiver.h>
#include <alljoyn_unity/MsgArg.h>
#include <alljoyn_unity/Session.h>

#include <Status.h>

#ifdef __cplusplus
extern "C" {
#endif

typedef struct _alljoyn_proxybusobject_handle*              alljoyn_proxybusobject;
#ifndef _ALLJOYN_OPAQUE_BUSATTACHMENT_
#define _ALLJOYN_OPAQUE_BUSATTACHMENT_
typedef struct _alljoyn_busattachment_handle*               alljoyn_busattachment;
#endif

/**
 * Create an empty proxy object that refers to an object at given remote service name. Note
 * that the created proxy object does not contain information about the interfaces that the
 * actual remote object implements with the exception that org.freedesktop.DBus.Peer
 * interface is special-cased (per the DBus spec) and can always be called on any object. Nor
 * does it contain information about the child objects that the actual remote object might
 * contain.
 *
 * To fill in this object with the interfaces and child object names that the actual remote
 * object describes in its introspection data, call IntrospectRemoteObject() or
 * IntrospectRemoteObjectAsync().
 *
 * @param bus        The bus.
 * @param service    The remote service name (well-known or unique).
 * @param path       The absolute (non-relative) object path for the remote object.
 * @param sessionId  The session id the be used for communicating with remote object.
 */
extern AJ_API alljoyn_proxybusobject alljoyn_proxybusobject_create(alljoyn_busattachment bus, const char* service,
                                                                   const char* path, alljoyn_sessionid sessionId);

/**
 * Destroy a proxy object created using alljoyn_proxybusobject_create.
 *
 * @param bus The bus object to destroy.
 */
extern AJ_API void alljoyn_proxybusobject_destroy(alljoyn_proxybusobject bus);

/**
 * Add an interface to this ProxyBusObject.
 *
 * Occasionally, AllJoyn library user may wish to call a method on
 * a %ProxyBusObject that was not reported during introspection of the remote obejct.
 * When this happens, the InterfaceDescription will have to be registered with the
 * Bus manually and the interface will have to be added to the %ProxyBusObject using this method.
 * @remark
 * The interface added via this call must have been previously registered with the
 * Bus. (i.e. it must have come from a call to alljoyn_busattachment_getinterface).
 *
 * @param bus      The bus object onto which the interface is to be added.
 * @param iface    The interface to add to this object. Must come from alljoyn_busattachment_getinterface.
 * @return
 *      - #ER_OK if successful.
 *      - An error status otherwise
 */
extern AJ_API QStatus alljoyn_proxybusobject_addinterface(alljoyn_proxybusobject bus, const alljoyn_interfacedescription iface);

/**
 * Make a synchronous method call
 *
 * @param obj          ProxyBusObject on which to call the method.
 * @param ifaceName    Name of interface.
 * @param methodName   Name of method.
 * @param args         The arguments for the method call (can be NULL)
 * @param numArgs      The number of arguments
 * @param replyMsg     The reply message received for the method call
 * @param timeout      Timeout specified in milliseconds to wait for a reply
 * @param flags        Logical OR of the message flags for this method call. The following flags apply to method calls:
 *                     - If #ALLJOYN_FLAG_ENCRYPTED is set the message is authenticated and the payload if any is encrypted.
 *                     - If #ALLJOYN_FLAG_COMPRESSED is set the header is compressed for destinations that can handle header compression.
 *                     - If #ALLJOYN_FLAG_AUTO_START is set the bus will attempt to start a service if it is not running.
 *
 * @return
 *      - #ER_OK if the method call succeeded and the reply message type is #MESSAGE_METHOD_RET
 *      - #ER_BUS_REPLY_IS_ERROR_MESSAGE if the reply message type is #MESSAGE_ERROR
 */
extern AJ_API QStatus alljoyn_proxybusobject_methodcall_synch(alljoyn_proxybusobject obj,
                                                              const char* ifaceName,
                                                              const char* methodName,
                                                              alljoyn_msgargs args,
                                                              size_t numArgs,
                                                              alljoyn_message replyMsg,
                                                              uint32_t timeout,
                                                              uint8_t flags);

#ifdef __cplusplus
} /* extern "C" */
#endif

#endif
