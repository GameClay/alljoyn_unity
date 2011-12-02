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
#include <string.h>
#include <assert.h>
#include "DeferredCallback.h"

namespace ajn {

/**
 * Abstract base class implemented by AllJoyn users and called by AllJoyn to inform
 * users of session port related events.
 */
class SessionPortListenerCallbackC : public SessionPortListener {
  public:
    SessionPortListenerCallbackC(const alljoyn_sessionportlistener_callbacks* in_callbacks, const void* in_context)
    {
        memcpy(&callbacks, in_callbacks, sizeof(alljoyn_sessionportlistener_callbacks));
        context = in_context;
    }

    virtual bool AcceptSessionJoiner(SessionPort sessionPort, const char* joiner, const SessionOpts& opts)
    {
        QC_BOOL ret = SessionPortListener::AcceptSessionJoiner(sessionPort, joiner, opts) ? QC_TRUE : QC_FALSE;
        if (callbacks.accept_session_joiner != NULL) {
            ret = callbacks.accept_session_joiner(context, sessionPort, joiner, (alljoyn_sessionopts)(&opts));
            DeferredCallback_4<QC_BOOL, const void*, SessionPort, const char*, alljoyn_sessionopts>* dcb =
                new DeferredCallback_4<QC_BOOL, const void*, SessionPort, const char*, alljoyn_sessionopts>(callbacks.accept_session_joiner, context, sessionPort, joiner, (alljoyn_sessionopts)(&opts));
            ret = dcb->Execute();
        }
        return (ret == QC_FALSE ? false : true);
    }

    virtual void SessionJoined(SessionPort sessionPort, SessionId id, const char* joiner)
    {
        if (callbacks.session_joined != NULL) {
            DeferredCallback_4<void, const void*, SessionPort, SessionId, const char*>* dcb =
                new DeferredCallback_4<void, const void*, SessionPort, SessionId, const char*>(callbacks.session_joined, context, sessionPort, id, joiner);
            dcb->Execute();
        }
    }
  protected:
    alljoyn_sessionportlistener_callbacks callbacks;
    const void* context;
};

}

struct _alljoyn_sessionportlistener_handle {
    /* Empty by design, this is just to allow the type restrictions to save coders from themselves */
};

alljoyn_sessionportlistener alljoyn_sessionportlistener_create(const alljoyn_sessionportlistener_callbacks* callbacks, const void* context)
{
    return (alljoyn_sessionportlistener) new ajn::SessionPortListenerCallbackC(callbacks, context);
}

void alljoyn_sessionportlistener_destroy(alljoyn_sessionportlistener listener)
{
    assert(listener != NULL && "listener parameter must not be NULL");
    delete (ajn::SessionPortListenerCallbackC*)listener;
}
