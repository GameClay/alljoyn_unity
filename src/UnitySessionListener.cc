/**
 * @file
 * This implements the Unity accessable version of the SessionListener class using
 * Mono delegates, and a pass-through implementation of SessionListener.
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

#include <alljoyn/SessionListener.h>
#include <alljoyn_unity/SessionListener.h>
#include "Unity.h"

namespace ajn {

/**
 * Abstract base class implemented by AllJoyn users and called by AllJoyn to inform
 * users of session related events.
 */
class UnitySessionListener : public SessionListener {
  public:
    UnitySessionListener(
        MonoObject* thisObj,
        MonoObject* sessionLostDelegate,
        MonoObject* sessionMemberAddedDelegate,
        MonoObject* sessionMemberRemovedDelegate) :
        this_object(thisObj),
        session_lost(sessionLostDelegate),
        session_member_added(sessionMemberAddedDelegate),
        session_member_removed(sessionMemberRemovedDelegate)
    {
        gchandles[0] = unity_mono.mono_gchandle_new(session_lost, 0);
        gchandles[1] = unity_mono.mono_gchandle_new(session_member_added, 0);
        gchandles[2] = unity_mono.mono_gchandle_new(session_member_removed, 0);

        thishandle = unity_mono.mono_gchandle_new(thisObj, 0);
    }

    virtual ~UnitySessionListener()
    {
        for (int i = 0; i < 3; i++) {
            unity_mono.mono_gchandle_free(gchandles[i]);
        }
        unity_mono.mono_gchandle_free(thishandle);
    }

    virtual void SessionLost(SessionId sessionId)
    {
        AJ_DEBUG_LOG("SessionLost %p\n", session_lost);
        MonoThread* mthread = unity_mono.mono_thread_attach(g_unity_mono_domain);
        AJ_DEBUG_LOG("mthread = %p\n", mthread);
        if (session_lost != NULL) {
            void* params[] = {this_object, &sessionId};
            unity_mono.mono_runtime_delegate_invoke(session_lost, params, NULL);
        }
    }

    virtual void SessionMemberAdded(SessionId sessionId, const char* uniqueName)
    {
        AJ_DEBUG_LOG("SessionMemberAdded %p\n", session_member_added);
        MonoThread* mthread = unity_mono.mono_thread_attach(g_unity_mono_domain);
        AJ_DEBUG_LOG("mthread = %p\n", mthread);
        if (session_member_added != NULL) {
            void* params[] = {this_object, &sessionId, &uniqueName};
            unity_mono.mono_runtime_delegate_invoke(session_member_added, params, NULL);
        }
    }

    virtual void SessionMemberRemoved(SessionId sessionId, const char* uniqueName)
    {
        AJ_DEBUG_LOG("SessionMemberRemoved %p\n", session_member_removed);
        MonoThread* mthread = unity_mono.mono_thread_attach(g_unity_mono_domain);
        AJ_DEBUG_LOG("mthread = %p\n", mthread);
        if (session_member_removed != NULL) {
            void* params[] = {this_object, &sessionId, &uniqueName};
            unity_mono.mono_runtime_delegate_invoke(session_member_removed, params, NULL);
        }
    }

  protected:
    MonoObject* this_object;
    MonoObject* session_lost;
    MonoObject* session_member_added;
    MonoObject* session_member_removed;

    uint32_t gchandles[3];
    uint32_t thishandle;
};

}

extern "C" {

alljoyn_sessionlistener alljoyn_unitysessionlistener_create(
    MonoObject* thisObj,
    MonoObject* sessionLostDelegate,
    MonoObject* sessionMemberAddedDelegate,
    MonoObject* sessionMemberRemovedDelegate)
{
    return (alljoyn_sessionlistener) new ajn::UnitySessionListener(thisObj, sessionLostDelegate,
        sessionMemberAddedDelegate, sessionMemberRemovedDelegate);
}

void alljoyn_unitysessionlistener_destroy(alljoyn_sessionlistener listener)
{
    if (listener != NULL) delete (ajn::UnitySessionListener*)listener;
}

} /* extern "C" */
