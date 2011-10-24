/**
 * @file
 * BusAttachment is the top-level object responsible for connecting to and optionally managing a message bus.
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
#include <qcc/Debug.h>
#include <qcc/Util.h>
#include <qcc/Event.h>
#include <qcc/String.h>
#include <qcc/Timer.h>
#include <qcc/atomic.h>
#include <qcc/XmlElement.h>
#include <qcc/StringSource.h>
#include <qcc/FileStream.h>

#include <assert.h>
#include <algorithm>

#include <alljoyn/BusAttachment.h>
#include <alljoyn/BusListener.h>
#include <alljoyn/DBusStd.h>
#include <alljoyn/AllJoynStd.h>

#define QCC_MODULE "ALLJOYN"

struct _alljoyn_busattachment_handle {
    /* Empty by design, this is just to allow the type restrictions to save coders from themselves */
};

alljoyn_busattachment alljoyn_busattachment_create(const char* applicationName, QC_BOOL allowRemoteMessages)
{
    bool allowRemoteMessagesBool = (allowRemoteMessages == QC_TRUE ? true : false);
    return ((alljoyn_busattachment) new ajn::BusAttachment(applicationName, allowRemoteMessagesBool));
}

void alljoyn_busattachment_destroy(alljoyn_busattachment bus)
{
    assert(bus != NULL && "NULL parameter passed to alljoyn_destroy_busattachment.");

    delete (ajn::BusAttachment*)bus;
}

QStatus alljoyn_busattachment_stop(alljoyn_busattachment bus, QC_BOOL blockUntilStopped)
{
    bool blockBool = (blockUntilStopped == QC_TRUE ? true : false);
    return ((ajn::BusAttachment*)bus)->Stop(blockBool);
}

QStatus alljoyn_busattachment_createinterface(alljoyn_busattachment bus,
                                              const char* name,
                                              alljoyn_interfacedescription* iface, QC_BOOL secure)
{
    bool secureBool = (secure == QC_TRUE ? true : false);
    ajn::InterfaceDescription* ifaceObj = NULL;
    QStatus ret = ((ajn::BusAttachment*)bus)->CreateInterface(name, ifaceObj, secureBool);
    *iface = (alljoyn_interfacedescription)ifaceObj;

    return ret;
}

QStatus alljoyn_busattachment_start(alljoyn_busattachment bus)
{
    return ((ajn::BusAttachment*)bus)->Start();
}

QStatus alljoyn_busattachment_connect(alljoyn_busattachment bus, const char* connectSpec)
{
    // Because the second parameter to Connect is only used internally it is not exposed to the C interface.
    return ((ajn::BusAttachment*)bus)->Connect(connectSpec, NULL);
}

void alljoyn_busattachment_registerbuslistener(alljoyn_busattachment bus, alljoyn_buslistener listener)
{
    ((ajn::BusAttachment*)bus)->RegisterBusListener((*(ajn::BusListener*)listener));
}

void alljoyn_busattachment_unregisterbuslistener(alljoyn_busattachment bus, alljoyn_buslistener listener)
{
    ((ajn::BusAttachment*)bus)->UnregisterBusListener((*(ajn::BusListener*)listener));
}

QStatus alljoyn_busattachment_findadvertisedname(alljoyn_busattachment bus, const char* namePrefix)
{
    return ((ajn::BusAttachment*)bus)->FindAdvertisedName(namePrefix);
}

QStatus alljoyn_busattachment_cancelfindadvertisedname(alljoyn_busattachment bus, const char* namePrefix)
{
    return ((ajn::BusAttachment*)bus)->CancelFindAdvertisedName(namePrefix);
}

const alljoyn_interfacedescription alljoyn_busattachment_getinterface(alljoyn_busattachment bus, const char* name)
{
    return (alljoyn_interfacedescription)((ajn::BusAttachment*)bus)->GetInterface(name);
}

QStatus alljoyn_busattachment_joinsession(alljoyn_busattachment bus, const char* sessionHost,
                                          alljoyn_sessionport sessionPort, alljoyn_buslistener listener,
                                          alljoyn_sessionid* sessionId, alljoyn_sessionopts opts)
{
    return ((ajn::BusAttachment*)bus)->JoinSession(sessionHost, (ajn::SessionPort)sessionPort,
                                                   (ajn::SessionListener*)listener, *((ajn::SessionId*)sessionId),
                                                   *((ajn::SessionOpts*)opts));
}

QStatus alljoyn_busattachment_registerbusobject(alljoyn_busattachment bus, alljoyn_busobject obj)
{
    return ((ajn::BusAttachment*)bus)->RegisterBusObject(*((ajn::BusObject*)obj));
}

void alljoyn_busattachment_unregisterbusobject(alljoyn_busattachment bus, alljoyn_busobject object)
{
    ((ajn::BusAttachment*)bus)->UnregisterBusObject(*((ajn::BusObject*)object));
}

void alljoyn_busattachment_waitstop(alljoyn_busattachment bus)
{
    ((ajn::BusAttachment*)bus)->WaitStop();
}

QStatus alljoyn_busattachment_requestname(alljoyn_busattachment bus, const char* requestedName, uint32_t flags)
{
    return ((ajn::BusAttachment*)bus)->RequestName(requestedName, flags);
}

QStatus alljoyn_busattachment_bindsessionport(alljoyn_busattachment bus, alljoyn_sessionport* sessionPort,
                                              const alljoyn_sessionopts opts, alljoyn_sessionportlistener listener)
{
    return ((ajn::BusAttachment*)bus)->BindSessionPort(*((ajn::SessionPort*)sessionPort),
                                                       *((const ajn::SessionOpts*)opts),
                                                       *((ajn::SessionPortListener*)listener));
}

QStatus alljoyn_busattachment_unbindsessionport(alljoyn_busattachment bus, alljoyn_sessionport sessionPort)
{
    return ((ajn::BusAttachment*)bus)->UnbindSessionPort(sessionPort);
}

QStatus alljoyn_busattachment_advertisename(alljoyn_busattachment bus, const char* name, alljoyn_transportmask transports)
{
    return ((ajn::BusAttachment*)bus)->AdvertiseName(name, transports);
}

QStatus alljoyn_busattachment_canceladvertisedname(alljoyn_busattachment bus, const char* name, alljoyn_transportmask transports)
{
    return ((ajn::BusAttachment*)bus)->CancelAdvertiseName(name, transports);
}

QStatus alljoyn_busattachment_enablepeersecurity(alljoyn_busattachment bus, const char* authMechanisms,
                                                 alljoyn_authlistener listener, const char* keyStoreFileName,
                                                 QC_BOOL isShared)
{
    return ((ajn::BusAttachment*)bus)->EnablePeerSecurity(authMechanisms, (ajn::AuthListener*)listener, keyStoreFileName,
                                                          (isShared == QC_TRUE ? true : false));
}

QC_BOOL alljoyn_busattachment_ispeersecurityenabled(alljoyn_busattachment bus)
{
    return (((ajn::BusAttachment*)bus)->IsPeerSecurityEnabled() == true ? QC_TRUE : QC_FALSE);
}

QStatus alljoyn_busattachment_createinterfacesfromxml(alljoyn_busattachment bus, const char* xml)
{
    return ((ajn::BusAttachment*)bus)->CreateInterfacesFromXml(xml);
}

size_t alljoyn_busattachment_getinterfaces(const alljoyn_busattachment bus,
                                           const alljoyn_interfacedescription* ifaces, size_t numIfaces)
{
    return ((ajn::BusAttachment*)bus)->GetInterfaces((const ajn::InterfaceDescription**)ifaces, numIfaces);
}

QStatus alljoyn_busattachment_deleteinterface(alljoyn_busattachment bus, alljoyn_interfacedescription iface)
{
    return ((ajn::BusAttachment*)bus)->DeleteInterface(*((ajn::InterfaceDescription*)iface));
}

QC_BOOL alljoyn_busattachment_isstarted(alljoyn_busattachment bus)
{
    return (((ajn::BusAttachment*)bus)->IsStarted() == true ? QC_TRUE : QC_FALSE);
}

QC_BOOL alljoyn_busattachment_isstopping(alljoyn_busattachment bus)
{
    return (((ajn::BusAttachment*)bus)->IsStopping() == true ? QC_TRUE : QC_FALSE);
}

QC_BOOL alljoyn_busattachment_isconnected(const alljoyn_busattachment bus)
{
    return (((const ajn::BusAttachment*)bus)->IsConnected() == true ? QC_TRUE : QC_FALSE);
}

QStatus alljoyn_busattachment_disconnect(alljoyn_busattachment bus, const char* connectSpec)
{
    return ((ajn::BusAttachment*)bus)->Disconnect(connectSpec);
}

const alljoyn_proxybusobject alljoyn_busattachment_getdbusproxyobj(alljoyn_busattachment bus)
{
    return (const alljoyn_proxybusobject)(&((ajn::BusAttachment*)bus)->GetDBusProxyObj());
}

const alljoyn_proxybusobject alljoyn_busattachment_getalljoynproxyobj(alljoyn_busattachment bus)
{
    return (const alljoyn_proxybusobject)(&((ajn::BusAttachment*)bus)->GetAllJoynProxyObj());
}

const alljoyn_proxybusobject alljoyn_busattachment_getalljoyndebugobj(alljoyn_busattachment bus)
{
    return (const alljoyn_proxybusobject)(&((ajn::BusAttachment*)bus)->GetAllJoynDebugObj());
}

const char* alljoyn_busattachment_getuniquename(const alljoyn_busattachment bus)
{
    return ((const ajn::BusAttachment*)bus)->GetUniqueName().c_str();
}

const char* alljoyn_busattachment_getglobalguidstring(const alljoyn_busattachment bus)
{
    return ((const ajn::BusAttachment*)bus)->GetGlobalGUIDString().c_str();
}

QStatus alljoyn_busattachment_registerkeystorelistener(alljoyn_busattachment bus, alljoyn_keystorelistener listener)
{
    return ((ajn::BusAttachment*)bus)->RegisterKeyStoreListener(*((ajn::KeyStoreListener*)listener));
}

QStatus alljoyn_busattachment_reloadkeystore(alljoyn_busattachment bus)
{
    return ((ajn::BusAttachment*)bus)->ReloadKeyStore();
}

void alljoyn_busattachment_clearkeystore(alljoyn_busattachment bus)
{
    ((ajn::BusAttachment*)bus)->ClearKeyStore();
}

QStatus alljoyn_busattachment_clearkeys(alljoyn_busattachment bus, const char* guid)
{
    return ((ajn::BusAttachment*)bus)->ClearKeys(guid);
}

QStatus alljoyn_busattachment_setkeyexpiration(alljoyn_busattachment bus, const char* guid, uint32_t timeout)
{
    return ((ajn::BusAttachment*)bus)->SetKeyExpiration(guid, timeout);
}

QStatus alljoyn_busattachment_getkeyexpiration(alljoyn_busattachment bus, const char* guid, uint32_t* timeout)
{
    return ((ajn::BusAttachment*)bus)->GetKeyExpiration(guid, *timeout);
}

QStatus alljoyn_busattachment_addlogonentry(alljoyn_busattachment bus, const char* authMechanism,
                                            const char* userName, const char* password)
{
    return ((ajn::BusAttachment*)bus)->AddLogonEntry(authMechanism, userName, password);
}

QStatus alljoyn_busattachment_releasename(alljoyn_busattachment bus, const char* name)
{
    return ((ajn::BusAttachment*)bus)->ReleaseName(name);
}

QStatus alljoyn_busattachment_addmatch(alljoyn_busattachment bus, const char* rule)
{
    return ((ajn::BusAttachment*)bus)->AddMatch(rule);
}

QStatus alljoyn_busattachment_removematch(alljoyn_busattachment bus, const char* rule)
{
    return ((ajn::BusAttachment*)bus)->RemoveMatch(rule);
}

QStatus alljoyn_busattachment_setsessionlistener(alljoyn_busattachment bus, alljoyn_sessionid sessionId,
                                                 alljoyn_sessionlistener listener)
{
    return ((ajn::BusAttachment*)bus)->SetSessionListener(sessionId, (ajn::SessionListener*)listener);
}

QStatus alljoyn_busattachment_leavesession(alljoyn_busattachment bus, alljoyn_sessionid sessionId)
{
    return ((ajn::BusAttachment*)bus)->LeaveSession(sessionId);
}

QStatus alljoyn_busattachment_setlinktimeout(alljoyn_busattachment bus, alljoyn_sessionid sessionid, uint32_t* linkTimeout)
{
    return ((ajn::BusAttachment*)bus)->SetLinkTimeout(sessionid, *linkTimeout);
}

QStatus alljoyn_busattachment_namehasowner(alljoyn_busattachment bus, const char* name, QC_BOOL* hasOwner)
{
    bool result;
    QStatus ret = ((ajn::BusAttachment*)bus)->NameHasOwner(name, result);
    *hasOwner = (result == true ? QC_TRUE : QC_FALSE);
    return ret;
}

QStatus alljoyn_busattachment_getpeerguid(alljoyn_busattachment bus, const char* name, char* guid, size_t guidSz)
{
    qcc::String guidStr;
    QStatus ret = ((ajn::BusAttachment*)bus)->GetPeerGUID(name, guidStr);
    strncpy(guid, guidStr.c_str(), guidSz);
    return ret;
}

QStatus alljoyn_busattachment_setdaemondebug(alljoyn_busattachment bus, const char* module, uint32_t level)
{
    return ((ajn::BusAttachment*)bus)->SetDaemonDebug(module, level);
}

uint32_t alljoyn_busattachment_gettimestamp()
{
    return ajn::BusAttachment::GetTimestamp();
}
