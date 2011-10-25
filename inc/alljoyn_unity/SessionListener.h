/**
 * @file
 * SessionListener is an abstract base class (interface) implemented by users of the
 * AllJoyn API in order to receive sessions related event information.
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
#ifndef _ALLJOYN_UNITY_SESSIONLISTENER_H
#define _ALLJOYN_UNITY_SESSIONLISTENER_H

#include <alljoyn_unity/AjAPI.h>
#include <alljoyn_unity/Session.h>

#ifdef __cplusplus
extern "C" {
#endif

typedef struct _alljoyn_sessionlistener_handle*             alljoyn_sessionlistener;

/**
 * Type for the SesionLost callback.
 */
typedef void (*alljoyn_sessionlistener_sessionlost_ptr)(const void* context, alljoyn_sessionid sessionId);

/**
 * Type for the SessionMemberAdded callback.
 */
typedef void (*alljoyn_sessionlistener_sessionmemberadded_ptr)(const void* context, alljoyn_sessionid sessionId,
                                                               const char* uniqueName);

/**
 * Type for the SessionMemberRemoved callback.
 */
typedef void (*alljoyn_sessionlistener_sessionmemberremoved_ptr)(const void* context, alljoyn_sessionid sessionId,
                                                                 const char* uniqueName);

/**
 * Structure used during alljoyn_sessionlistener_create to provide callbacks into C.
 */
typedef struct {
    alljoyn_sessionlistener_sessionlost_ptr session_lost;
    alljoyn_sessionlistener_sessionmemberadded_ptr session_member_added;
    alljoyn_sessionlistener_sessionmemberremoved_ptr session_member_removed;
} alljoyn_sessionlistener_callbacks;

/**
 * Create a SessionListener which will trigger the provided callbacks, passing along the provided context.
 *
 * @param callbacks Callbacks to trigger for associated events.
 * @param context   Context to pass to callback functions
 *
 * @return Handle to newly allocated SessionListener.
 */
extern AJ_API alljoyn_sessionlistener alljoyn_sessionlistener_create(const alljoyn_sessionlistener_callbacks* callbacks,
                                                                     const void* context);

/**
 * Destroy a SessionListener.
 *
 * @param listener SessionListener to destroy.
 */
extern AJ_API void alljoyn_sessionlistener_destroy(alljoyn_sessionlistener listener);

#ifdef __cplusplus
} /* extern "C" */
#endif

#endif
