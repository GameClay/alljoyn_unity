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
#ifndef _ALLJOYN_UNITY_H
#define _ALLJOYN_UNITY_H

#ifdef __cplusplus
extern "C" {
#endif

#include <alljoyn_unity/AjAPI.h>
#include <Status.h>
#include <stdint.h>

#ifdef QCC_OS_ANDROID
#   include <android/log.h>
#   define AJ_DEBUG_LOG(s, ...) __android_log_print(ANDROID_LOG_DEBUG, "Unity", s, ## __VA_ARGS__)
#else
#   include <stdio.h>
#   define AJ_DEBUG_LOG(s, ...) printf(s, ## __VA_ARGS__)
#endif

typedef void* MonoObject;
typedef void* MonoDomain;
typedef void* MonoThread;
typedef void* MonoClass;

typedef MonoObject* (*mono_runtime_delegate_invoke_ptr)(MonoObject* delegate, void** params, MonoObject** exc);
typedef void (*mono_add_internal_call_ptr)(const char *name, const void* method);
typedef MonoDomain* (*mono_domain_get_ptr)();
typedef MonoThread* (*mono_thread_attach_ptr)(MonoDomain* domain);
typedef MonoThread* (*mono_thread_current_ptr)();
typedef uint32_t (*mono_gchandle_new_ptr)(MonoObject* object, int pinned);
typedef void (*mono_gchandle_free_ptr)(uint32_t handle);
typedef void* (*mono_object_unbox_ptr)(MonoObject *obj);

typedef struct {
    mono_runtime_delegate_invoke_ptr mono_runtime_delegate_invoke;
    mono_add_internal_call_ptr mono_add_internal_call;
    mono_domain_get_ptr mono_domain_get;
    mono_thread_attach_ptr mono_thread_attach;
    mono_thread_current_ptr mono_thread_current;
    mono_gchandle_new_ptr mono_gchandle_new;
    mono_gchandle_free_ptr mono_gchandle_free;
    mono_object_unbox_ptr mono_object_unbox;
} mono_iface;

extern mono_iface unity_mono;

extern MonoDomain* g_unity_mono_domain;

extern void* PNULL;

extern AJ_API QStatus alljoyn_unity_initialize(const char* libMonoPath);

extern AJ_API void alljoyn_unity_destroy();

#ifdef __cplusplus
} /* extern "C" */
#endif

#endif
