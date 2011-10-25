/**
 * @file
 * SessionPortListener is an abstract base class (interface) implemented by users of the
 * AllJoyn API in order to receive session port related event information.
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
#ifndef _ALLJOYN_UNITY_SESSIONPORTLISTENER_H
#define _ALLJOYN_UNITY_SESSIONPORTLISTENER_H

#include <alljoyn_unity/AjAPI.h>
#include <alljoyn_unity/Session.h>
#include <alljoyn_unity/SessionListener.h>

#ifdef __cplusplus
extern "C" {
#endif

typedef struct _alljoyn_sessionportlistener_handle*         alljoyn_sessionportlistener;

/**
 * Type for the AcceptSessionJoiner callback.
 */
typedef QC_BOOL (*alljoyn_sessionportlistener_acceptsessionjoiner_ptr)(const void* context, alljoyn_sessionport sessionPort,
                                                                       const char* joiner,  const alljoyn_sessionopts opts);

/**
 * Type for the SessionJoined callback.
 */
typedef void (*alljoyn_sessionportlistener_sessionjoined_ptr)(const void* context, alljoyn_sessionport sessionPort,
                                                              alljoyn_sessionid id, const char* joiner);

/**
 * Structure used during alljoyn_sessionportlistener_create to provide callbacks into C.
 */
typedef struct {
    alljoyn_sessionportlistener_acceptsessionjoiner_ptr accept_session_joiner;
    alljoyn_sessionportlistener_sessionjoined_ptr session_joined;
} alljoyn_sessionportlistener_callbacks;

/**
 * Create a SessionPortListener which will trigger the provided callbacks, passing along the provided context.
 *
 * @param callbacks Callbacks to trigger for associated events.
 * @param context   Context to pass to callback functions
 *
 * @return Handle to newly allocated SessionPortListener.
 */
extern AJ_API alljoyn_sessionportlistener alljoyn_sessionportlistener_create(const alljoyn_sessionportlistener_callbacks* callbacks,
                                                                             const void* context);

/**
 * Destroy a SessionPortListener.
 *
 * @param listener SessionPortListener to destroy.
 */
extern AJ_API void alljoyn_sessionportlistener_destroy(alljoyn_sessionportlistener listener);

#ifdef __cplusplus
} /* extern "C" */
#endif

#endif
