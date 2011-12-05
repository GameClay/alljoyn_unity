#ifndef _ALLJOYN_UNITY_DEFERREDCALLBACK_H
#define _ALLJOYN_UNITY_DEFERREDCALLBACK_H

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

#include <alljoyn_unity/AjAPI.h>
#include <list>
#include <signal.h>
#include <pthread.h>
#include <unistd.h>
#include <qcc/Mutex.h>

//#define DEBUG_DEFERRED_CALLBACKS 1

#if DEBUG_DEFERRED_CALLBACKS
#   include <stdio.h>
#   define DEFERRED_CALLBACK_EXECUTE(cb) cb->Execute(); printf("%s (%d) -- Executing on %s thread\n", __FILE__, __LINE__, DeferredCallback::IsMainThread() ? "main" : "alternate")
#else
#   define DEFERRED_CALLBACK_EXECUTE(cb) cb->Execute()
#endif

namespace ajn {

class DeferredCallback {
  public:
    DeferredCallback() : executeNow(false), finished(false) { }

    virtual ~DeferredCallback() { }

    static int TriggerCallbacks()
    {
        int ret = 0;
        while (!sPendingCallbacks.empty()) {
            sCallbackListLock.Lock(MUTEX_CONTEXT);
            DeferredCallback* cb = sPendingCallbacks.front();
            sPendingCallbacks.pop_front();
            sCallbackListLock.Unlock(MUTEX_CONTEXT);
            cb->executeNow = true;
            while (!cb->finished)
                usleep(1);
            delete cb;
            ret++;
        }
        return ret;
    }

    static bool IsMainThread()
    {
        return (sMainThreadCallbacksOnly ? (pthread_equal(sMainThread, pthread_self()) != 0) : true);
    }

  protected:
    void Wait()
    {
        while (!executeNow)
            usleep(1);
    }

    class ScopeFinishedMarker {
      public:
        ScopeFinishedMarker(volatile sig_atomic_t* finished) : finishedSig(finished) { }
        ~ScopeFinishedMarker() { *finishedSig = true; }
      private:
        volatile sig_atomic_t* finishedSig;
    };

  private:
    volatile sig_atomic_t executeNow;
    static qcc::Mutex sCallbackListLock;

  protected:
    volatile sig_atomic_t finished;
    static std::list<DeferredCallback*> sPendingCallbacks;
    static pthread_t sMainThread;

  public:
    static bool sMainThreadCallbacksOnly;
};

template <typename R, typename T>
class DeferredCallback_1 : public DeferredCallback {
  public:
    typedef R (*DeferredCallback_1_Callback)(T arg1);

    DeferredCallback_1(DeferredCallback_1_Callback callback, T param1) : _callback(callback), _param1(param1)
    {
    }

    virtual R Execute()
    {
        ScopeFinishedMarker finisher(&finished);
        sCallbackListLock.Lock(MUTEX_CONTEXT);
        sPendingCallbacks.push_back(this);
        sCallbackListLock.Unlock(MUTEX_CONTEXT);
        if (!IsMainThread()) Wait();
        return _callback(_param1);
    }

  protected:
    DeferredCallback_1_Callback _callback;
    T _param1;
};

template <typename R, typename T, typename U>
class DeferredCallback_2 : public DeferredCallback {
  public:
    typedef R (*DeferredCallback_2_Callback)(T arg1, U arg2);

    DeferredCallback_2(DeferredCallback_2_Callback callback, T param1, U param2) : _callback(callback), _param1(param1), _param2(param2)
    {
    }

    virtual R Execute()
    {
        ScopeFinishedMarker finisher(&finished);
        sCallbackListLock.Lock(MUTEX_CONTEXT);
        sPendingCallbacks.push_back(this);
        sCallbackListLock.Unlock(MUTEX_CONTEXT);
        if (!IsMainThread()) Wait();
        return _callback(_param1, _param2);
    }

  protected:
    DeferredCallback_2_Callback _callback;
    T _param1;
    U _param2;
};

template <typename R, typename T, typename U, typename V>
class DeferredCallback_3 : public DeferredCallback {
  public:
    typedef R (*DeferredCallback_3_Callback)(T arg1, U arg2, V arg3);

    DeferredCallback_3(DeferredCallback_3_Callback callback, T param1, U param2, V param3) :
        _callback(callback), _param1(param1), _param2(param2), _param3(param3)
    {
    }

    virtual R Execute()
    {
        ScopeFinishedMarker finisher(&finished);
        sCallbackListLock.Lock(MUTEX_CONTEXT);
        sPendingCallbacks.push_back(this);
        sCallbackListLock.Unlock(MUTEX_CONTEXT);
        if (!IsMainThread()) Wait();
        return _callback(_param1, _param2, _param3);
    }

  protected:
    DeferredCallback_3_Callback _callback;
    T _param1;
    U _param2;
    V _param3;
};

template <typename R, typename T, typename U, typename V, typename W>
class DeferredCallback_4 : public DeferredCallback {
  public:
    typedef R (*DeferredCallback_4_Callback)(T arg1, U arg2, V arg3, W arg4);

    DeferredCallback_4(DeferredCallback_4_Callback callback, T param1, U param2, V param3, W param4) :
        _callback(callback), _param1(param1), _param2(param2), _param3(param3), _param4(param4)
    {
    }

    virtual R Execute()
    {
        ScopeFinishedMarker finisher(&finished);
        sCallbackListLock.Lock(MUTEX_CONTEXT);
        sPendingCallbacks.push_back(this);
        sCallbackListLock.Unlock(MUTEX_CONTEXT);
        if (!IsMainThread()) Wait();
        return _callback(_param1, _param2, _param3, _param4);
    }

  protected:
    DeferredCallback_4_Callback _callback;
    T _param1;
    U _param2;
    V _param3;
    W _param4;
};

}

#endif
