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
#include "Unity.h"

namespace ajn {

/**
 * Abstract base class implemented by AllJoyn users and called by AllJoyn to inform
 * users of bus related events.
 */
class UnityBusListener : BusListener {
  public:
    UnityBusListener(
        MonoObject* thisObj,
        MonoObject* listenerRegisteredDelegate,
        MonoObject* listenerUnregisteredDelegate,
        MonoObject* foundAdvertisedNameDelegate,
        MonoObject* lostAdvertisedNameDelegate,
        MonoObject* nameOwnerChangedDelegate,
        MonoObject* busStoppingDelegate,
        MonoObject* busDisconnectedDelegate) :
        this_object(thisObj),
        listener_registered(listenerRegisteredDelegate),
        listener_unregistered(listenerUnregisteredDelegate),
        found_advertised_name(foundAdvertisedNameDelegate),
        lost_advertised_name(lostAdvertisedNameDelegate),
        name_owner_changed(nameOwnerChangedDelegate),
        bus_stopping(busStoppingDelegate),
        bus_disconnected(busDisconnectedDelegate)
    {
        gchandles[0] = unity_mono.mono_gchandle_new(listener_registered, 0);
        gchandles[1] = unity_mono.mono_gchandle_new(listener_unregistered, 0);
        gchandles[2] = unity_mono.mono_gchandle_new(found_advertised_name, 0);
        gchandles[3] = unity_mono.mono_gchandle_new(lost_advertised_name, 0);
        gchandles[4] = unity_mono.mono_gchandle_new(name_owner_changed, 0);
        gchandles[5] = unity_mono.mono_gchandle_new(bus_stopping, 0);
        gchandles[6] = unity_mono.mono_gchandle_new(bus_disconnected, 0);

        thishandle = unity_mono.mono_gchandle_new(this_object, 0);
    }

    virtual ~UnityBusListener()
    {
        for (int i = 0; i < 7; i++) {
            unity_mono.mono_gchandle_free(gchandles[i]);
        }
        unity_mono.mono_gchandle_free(thishandle);
    }

    void ListenerRegistered(BusAttachment* bus)
    {
        AJ_DEBUG_LOG("ListenerRegistered %p\n", listener_registered);
        MonoThread* mthread = unity_mono.mono_thread_attach(g_unity_mono_domain);
        AJ_DEBUG_LOG("mthread = %p\n", mthread);
        if (listener_registered != NULL) {
            void* params[] = {this_object, &bus};
            unity_mono.mono_runtime_delegate_invoke(listener_registered, params, NULL);
        }
    }

    void ListenerUnregistered()
    {
        AJ_DEBUG_LOG("ListenerUnregistered %p\n", listener_unregistered);
        MonoThread* mthread = unity_mono.mono_thread_attach(g_unity_mono_domain);
        AJ_DEBUG_LOG("mthread = %p\n", mthread);
        if (listener_unregistered != NULL) {
            void* params[] = {this_object};
            unity_mono.mono_runtime_delegate_invoke(listener_unregistered, params, NULL);
        }
    }

    void FoundAdvertisedName(const char* name, TransportMask transport, const char* namePrefix)
    {
        AJ_DEBUG_LOG("FoundAdvertisedName %p\n", found_advertised_name);
        MonoThread* mthread = unity_mono.mono_thread_attach(g_unity_mono_domain);
        AJ_DEBUG_LOG("mthread = %p\n", mthread);
        if (found_advertised_name != NULL) {
            void* params[] = {this_object, &name, &transport, &namePrefix};
            unity_mono.mono_runtime_delegate_invoke(found_advertised_name, params, NULL);
        }
    }

    void LostAdvertisedName(const char* name, TransportMask transport, const char* namePrefix)
    {
        AJ_DEBUG_LOG("LostAdvertisedName %p\n", lost_advertised_name);
        MonoThread* mthread = unity_mono.mono_thread_attach(g_unity_mono_domain);
        AJ_DEBUG_LOG("mthread = %p\n", mthread);
        if (lost_advertised_name != NULL) {
            void* params[] = {this_object, &name, &transport, &namePrefix};
            unity_mono.mono_runtime_delegate_invoke(lost_advertised_name, params, NULL);
        }
    }

    void NameOwnerChanged(const char* busName, const char* previousOwner, const char* newOwner)
    {
        AJ_DEBUG_LOG("NameOwnerChanged %p\n", name_owner_changed);
        MonoThread* mthread = unity_mono.mono_thread_attach(g_unity_mono_domain);
        AJ_DEBUG_LOG("mthread = %p\n", mthread);
        if (name_owner_changed != NULL) {
            void* params[] = {this_object, &busName, &previousOwner, &newOwner};
            unity_mono.mono_runtime_delegate_invoke(name_owner_changed, params, NULL);
        }
    }

    void BusStopping()
    {
        AJ_DEBUG_LOG("BusStopping %p\n", bus_stopping);
        MonoThread* mthread = unity_mono.mono_thread_attach(g_unity_mono_domain);
        AJ_DEBUG_LOG("mthread = %p\n", mthread);
        if (bus_stopping != NULL) {
            void* params[] = {this_object};
            unity_mono.mono_runtime_delegate_invoke(bus_stopping, params, NULL);
        }
    }

    void BusDisconnected()
    {
        AJ_DEBUG_LOG("BusDisconnected %p\n", bus_disconnected);
        MonoThread* mthread = unity_mono.mono_thread_attach(g_unity_mono_domain);
        AJ_DEBUG_LOG("mthread = %p\n", mthread);
        if (bus_disconnected != NULL) {
            void* params[] = {this_object};
            unity_mono.mono_runtime_delegate_invoke(bus_disconnected, params, NULL);
        }
    }

  protected:
    MonoObject* this_object;
    MonoObject* listener_registered;
    MonoObject* listener_unregistered;
    MonoObject* found_advertised_name;
    MonoObject* lost_advertised_name;
    MonoObject* name_owner_changed;
    MonoObject* bus_stopping;
    MonoObject* bus_disconnected;

    uint32_t gchandles[7];
    uint32_t thishandle;
};

}

extern "C" {

alljoyn_buslistener alljoyn_unitybuslistener_create(
    MonoObject* thisObj,
    MonoObject* listenerRegisteredDelegate,
    MonoObject* listenerUnregisteredDelegate,
    MonoObject* foundAdvertisedNameDelegate,
    MonoObject* lostAdvertisedNameDelegate,
    MonoObject* nameOwnerChangedDelegate,
    MonoObject* busStoppingDelegate,
    MonoObject* busDisconnectedDelegate)
{
    return (alljoyn_buslistener) new ajn::UnityBusListener(thisObj, listenerRegisteredDelegate,
        listenerUnregisteredDelegate, foundAdvertisedNameDelegate, lostAdvertisedNameDelegate,
        nameOwnerChangedDelegate, busStoppingDelegate, busDisconnectedDelegate);
}

void alljoyn_unitybuslistener_destroy(alljoyn_buslistener listener)
{
    delete (ajn::UnityBusListener*)listener;
}

} /* extern "C" */

