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

#include "DeferredCallback.h"

namespace ajn {

std::list<DeferredCallback*> DeferredCallback::sPendingCallbacks;
pthread_t DeferredCallback::sMainThread = pthread_self();
bool DeferredCallback::sMainThreadCallbacksOnly = false;
qcc::Mutex DeferredCallback::sCallbackListLock;

}

int alljoyn_unity_deferred_callbacks_process()
{
    return ajn::DeferredCallback::TriggerCallbacks();
}

void alljoin_unity_set_deferred_callback_mainthread_only(int mainthread_only)
{
    ajn::DeferredCallback::sMainThreadCallbacksOnly = (mainthread_only == 1 ? true : false);
}
