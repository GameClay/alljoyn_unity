/**
 * @file
 * This implements the C accessable version of the SessionPortListener class using
 * function pointers, and a pass-through implementation of SessionPortListener.
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

#include <alljoyn/SessionPortListener.h>
#include <alljoyn_unity/SessionPortListener.h>
#include "Unity.h"

namespace ajn {

/**
 * Abstract base class implemented by AllJoyn users and called by AllJoyn to inform
 * users of session port related events.
 */
class UnitySessionPortListener : public SessionPortListener {
  public:
    UnitySessionPortListener(
        MonoObject* thisObj,
        MonoObject* acceptSessionJoinerDelegate,
        MonoObject* sessionJoinedDelegate) :
        this_object(thisObj),
        accept_session_joiner(acceptSessionJoinerDelegate),
        session_joined(sessionJoinedDelegate)
    {
        gchandles[0] = unity_mono.mono_gchandle_new(accept_session_joiner, 0);
        gchandles[1] = unity_mono.mono_gchandle_new(session_joined, 0);

        thishandle = unity_mono.mono_gchandle_new(thisObj, 0);
    }

    virtual ~UnitySessionPortListener()
    {
        for (int i = 0; i < 2; i++) {
            unity_mono.mono_gchandle_free(gchandles[i]);
        }
        unity_mono.mono_gchandle_free(thishandle);
    }

    virtual bool AcceptSessionJoiner(SessionPort sessionPort, const char* joiner, const SessionOpts& opts)
    {
        AJ_DEBUG_LOG("AcceptSessionJoiner %p\n", accept_session_joiner);
        MonoThread* mthread = unity_mono.mono_thread_attach(g_unity_mono_domain);
        AJ_DEBUG_LOG("mthread = %p\n", mthread);
        QC_BOOL ret = SessionPortListener::AcceptSessionJoiner(sessionPort, joiner, opts) ? QC_TRUE : QC_FALSE;
        if (accept_session_joiner != NULL) {
            void* params[] = {this_object, &sessionPort, &joiner, (void*)&opts};
            MonoObject* retobj = NULL;
            MonoObject* except = NULL;
            retobj = unity_mono.mono_runtime_delegate_invoke(accept_session_joiner, params, &except);

            if (except == NULL) {
                ret = *(int*)unity_mono.mono_object_unbox(retobj);
                AJ_DEBUG_LOG("AcceptSessionJoiner %p returned %d\n", accept_session_joiner, ret);
            }
            else {
                AJ_DEBUG_LOG("AcceptSessionJoiner %p threw an exception.\n", accept_session_joiner);
            }
        }
        return (ret == QC_FALSE ? false : true);
    }

    virtual void SessionJoined(SessionPort sessionPort, SessionId id, const char* joiner)
    {
        AJ_DEBUG_LOG("SessionJoined %p\n", session_joined);
        MonoThread* mthread = unity_mono.mono_thread_attach(g_unity_mono_domain);
        AJ_DEBUG_LOG("mthread = %p\n", mthread);
        if (session_joined != NULL) {
            void* params[] = {this_object, &sessionPort, &id, &joiner};
            unity_mono.mono_runtime_delegate_invoke(session_joined, params, NULL);
        }
    }
  protected:
    MonoObject* this_object;
    MonoObject* accept_session_joiner;
    MonoObject* session_joined;

    uint32_t gchandles[2];
    uint32_t thishandle;
};

}

extern "C" {

alljoyn_sessionportlistener alljoyn_unitysessionportlistener_create(MonoObject* thisObj,
    MonoObject* acceptSessionJoinerDelegate, MonoObject* sessionJoinedDelegate)
{
    return (alljoyn_sessionportlistener) new ajn::UnitySessionPortListener(thisObj,
        acceptSessionJoinerDelegate, sessionJoinedDelegate);
}

void alljoyn_unitysessionportlistener_destroy(alljoyn_sessionportlistener listener)
{
    delete (ajn::UnitySessionPortListener*)listener;
}

} /* extern "C" */
