/**
 * @file
 * This implements the C accessable version of the SessionListener class using
 * function pointers, and a pass-through implementation of SessionListener.
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
#include <string.h>
#include <assert.h>
#include "DeferredCallback.h"

namespace ajn {

/**
 * Abstract base class implemented by AllJoyn users and called by AllJoyn to inform
 * users of session related events.
 */
class SessionListenerCallbackC : public SessionListener {
  public:
    SessionListenerCallbackC(const alljoyn_sessionlistener_callbacks* in_callbacks, const void* in_context)
    {
        memcpy(&callbacks, in_callbacks, sizeof(alljoyn_sessionlistener_callbacks));
        context = in_context;
    }

    virtual void SessionLost(SessionId sessionId)
    {
        if (callbacks.session_lost != NULL) {
            DeferredCallback_2<void, const void*, SessionId>* dcb =
                new DeferredCallback_2<void, const void*, SessionId>(callbacks.session_lost, context, sessionId);
            DEFERRED_CALLBACK_EXECUTE(dcb);
        }
    }

    virtual void SessionMemberAdded(SessionId sessionId, const char* uniqueName)
    {
        if (callbacks.session_member_added != NULL) {
            DeferredCallback_3<void, const void*, SessionId, const char*>* dcb =
                new DeferredCallback_3<void, const void*, SessionId, const char*>(callbacks.session_member_added, context, sessionId, uniqueName);
            DEFERRED_CALLBACK_EXECUTE(dcb);
        }
    }

    virtual void SessionMemberRemoved(SessionId sessionId, const char* uniqueName)
    {
        if (callbacks.session_member_removed != NULL) {
            DeferredCallback_3<void, const void*, SessionId, const char*>* dcb =
                new DeferredCallback_3<void, const void*, SessionId, const char*>(callbacks.session_member_removed, context, sessionId, uniqueName);
            DEFERRED_CALLBACK_EXECUTE(dcb);
        }
    }
  protected:
    alljoyn_sessionlistener_callbacks callbacks;
    const void* context;
};

}

struct _alljoyn_sessionlistener_handle {
    /* Empty by design, this is just to allow the type restrictions to save coders from themselves */
};

alljoyn_sessionlistener alljoyn_sessionlistener_create(const alljoyn_sessionlistener_callbacks* callbacks, const void* context)
{
    return (alljoyn_sessionlistener) new ajn::SessionListenerCallbackC(callbacks, context);
}

void alljoyn_sessionlistener_destroy(alljoyn_sessionlistener listener)
{
    assert(listener != NULL && "listener parameter must not be NULL");
    delete (ajn::SessionListenerCallbackC*)listener;
}
