/**
 * @file
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

#include <dlfcn.h>
#include "Unity.h"

#include <alljoyn_unity/BusListener.h>
#include <alljoyn_unity/SessionListener.h>
 #include <alljoyn_unity/SessionPortListener.h>

extern "C" {

MonoDomain* g_unity_mono_domain = NULL;
mono_iface unity_mono;

void* g_NULL = NULL;
void* PNULL = &g_NULL;

extern alljoyn_sessionlistener alljoyn_unitysessionlistener_create(MonoObject* sessionLostDelegate,
    MonoObject* sessionMemberAddedDelegate, MonoObject* sessionMemberRemovedDelegate);

extern alljoyn_buslistener alljoyn_unitybuslistener_create(
    MonoObject* listenerRegisteredDelegate,
    MonoObject* listenerUnregisteredDelegate,
    MonoObject* foundAdvertisedNameDelegate,
    MonoObject* lostAdvertisedNameDelegate,
    MonoObject* nameOwnerChangedDelegate,
    MonoObject* busStoppingDelegate,
    MonoObject* busDisconnectedDelegate);

extern alljoyn_sessionportlistener alljoyn_unitysessionportlistener_create(
    MonoObject* acceptSessionJoinerDelegate, MonoObject* sessionJoinedDelegate);
};

void* g_mono_lib_hdl = NULL;

QStatus alljoyn_unity_initialize(const char* libMonoPath)
{
    QStatus ret = ER_BUS_UNKNOWN_PATH;

    if(strcmp(libMonoPath, "") == 0) libMonoPath = NULL;
    g_mono_lib_hdl = dlopen(libMonoPath, RTLD_NOW);

    if (g_mono_lib_hdl != NULL) {
        void* fn_hdl = NULL;
        bool loaded_ptrs = true;

        /* If the function pointers can't be found, invalid address */
        ret = ER_INVALID_ADDRESS;

        /* mono_runtime_delegate_invoke */
        fn_hdl = dlsym(g_mono_lib_hdl, "mono_runtime_delegate_invoke");
        if(fn_hdl == NULL) {
            AJ_DEBUG_LOG("ARG! mono_runtime_delegate_invoke %s\n", dlerror());
        }
        loaded_ptrs &= (fn_hdl != NULL);
        unity_mono.mono_runtime_delegate_invoke = (mono_runtime_delegate_invoke_ptr)fn_hdl;

        /* mono_add_internal_call */
        fn_hdl = dlsym(g_mono_lib_hdl, "mono_add_internal_call");
        if(fn_hdl == NULL) {
            AJ_DEBUG_LOG("ARG! mono_add_internal_call %s\n", dlerror());
        }
        loaded_ptrs &= (fn_hdl != NULL);
        unity_mono.mono_add_internal_call = (mono_add_internal_call_ptr)fn_hdl;

        /* mono_domain_get */
        fn_hdl = dlsym(g_mono_lib_hdl, "mono_domain_get");
        if(fn_hdl == NULL) {
            AJ_DEBUG_LOG("ARG! mono_domain_get %s\n", dlerror());
        }
        loaded_ptrs &= (fn_hdl != NULL);
        unity_mono.mono_domain_get = (mono_domain_get_ptr)fn_hdl;

        /* mono_thread_attach */
        fn_hdl = dlsym(g_mono_lib_hdl, "mono_thread_attach");
        if(fn_hdl == NULL) {
            AJ_DEBUG_LOG("ARG! mono_thread_attach %s\n", dlerror());
        }
        loaded_ptrs &= (fn_hdl != NULL);
        unity_mono.mono_thread_attach = (mono_thread_attach_ptr)fn_hdl;

        /* mono_thread_detach */
        fn_hdl = dlsym(g_mono_lib_hdl, "mono_thread_detach");
        if(fn_hdl == NULL) {
            AJ_DEBUG_LOG("ARG! mono_thread_detach %s\n", dlerror());
        }
        loaded_ptrs &= (fn_hdl != NULL);
        unity_mono.mono_thread_detach = (mono_thread_detach_ptr)fn_hdl;

        /* mono_thread_current */
        fn_hdl = dlsym(g_mono_lib_hdl, "mono_thread_current");
        if(fn_hdl == NULL) {
            AJ_DEBUG_LOG("ARG! mono_thread_current %s\n", dlerror());
        }
        loaded_ptrs &= (fn_hdl != NULL);
        unity_mono.mono_thread_current = (mono_thread_current_ptr)fn_hdl;

        /* mono_gchandle_new */
        fn_hdl = dlsym(g_mono_lib_hdl, "mono_gchandle_new");
        if(fn_hdl == NULL) {
            AJ_DEBUG_LOG("ARG! mono_gchandle_new %s\n", dlerror());
        }
        loaded_ptrs &= (fn_hdl != NULL);
        unity_mono.mono_gchandle_new = (mono_gchandle_new_ptr)fn_hdl;

        /* mono_gchandle_free */
        fn_hdl = dlsym(g_mono_lib_hdl, "mono_gchandle_free");
        if(fn_hdl == NULL) {
            AJ_DEBUG_LOG("ARG! mono_gchandle_free %s\n", dlerror());
        }
        loaded_ptrs &= (fn_hdl != NULL);
        unity_mono.mono_gchandle_free = (mono_gchandle_free_ptr)fn_hdl;

        /* mono_object_unbox */
        fn_hdl = dlsym(g_mono_lib_hdl, "mono_object_unbox");
        if(fn_hdl == NULL) {
            AJ_DEBUG_LOG("ARG! mono_object_unbox %s\n", dlerror());
        }
        loaded_ptrs &= (fn_hdl != NULL);
        unity_mono.mono_object_unbox = (mono_object_unbox_ptr)fn_hdl;

        /* Register internal calls and save off the current domain */
        if (loaded_ptrs) {
            g_unity_mono_domain = unity_mono.mono_domain_get();

            unity_mono.mono_add_internal_call(
                "AllJoynUnity.AllJoyn/SessionListener::alljoyn_unitysessionlistener_create",
                (void*)alljoyn_unitysessionlistener_create);
            unity_mono.mono_add_internal_call(
                "AllJoynUnity.AllJoyn/BusListener::alljoyn_unitybuslistener_create",
                (void*)alljoyn_unitybuslistener_create);
            unity_mono.mono_add_internal_call(
                "AllJoynUnity.AllJoyn/SessionPortListener::alljoyn_unitysessionportlistener_create",
                (void*)alljoyn_unitysessionportlistener_create);

            ret = ER_OK;
        }
    }
    else {
        AJ_DEBUG_LOG("ARG! couldn't dlopen %s\n", dlerror());
    }

    return ret;
}

void alljoyn_unity_destroy()
{
    dlclose(g_mono_lib_hdl);
    g_mono_lib_hdl = NULL;
}
