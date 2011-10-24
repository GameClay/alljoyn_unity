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
#include <alljoyn/KeyStoreListener.h>
#include "KeyStore.h"
#include <string.h>
#include <assert.h>
#include <string.h>

namespace ajn {

/**
 * Abstract base class implemented by AllJoyn users and called by AllJoyn to inform
 * users of keystore related events.
 */
class KeyStoreListenerCallbackC : public KeyStoreListener {
  public:
    KeyStoreListenerCallbackC(const alljoyn_keystorelistener_callbacks* in_callbacks, const void* in_context)
    {
        memcpy(&callbacks, in_callbacks, sizeof(alljoyn_keystorelistener_callbacks));
        context = in_context;
    }

    virtual QStatus LoadRequest(KeyStore& keyStore)
    {
        assert(callbacks.load_request != NULL && "load_request callback required.");
        return callbacks.load_request(context, (alljoyn_keystore)(&keyStore));
    }

    virtual QStatus StoreRequest(KeyStore& keyStore)
    {
        assert(callbacks.store_request != NULL && "store_request callback required.");
        return callbacks.store_request(context, (alljoyn_keystore)(&keyStore));
    }
  protected:
    alljoyn_keystorelistener_callbacks callbacks;
    const void* context;
};

}

struct _alljoyn_keystorelistener_handle {
    /* Empty by design, this is just to allow the type restrictions to save coders from themselves */
};

alljoyn_keystorelistener alljoyn_keystorelistener_create(const alljoyn_keystorelistener_callbacks* callbacks, const void* context)
{
    assert(callbacks->load_request != NULL && "load_request callback required.");
    assert(callbacks->store_request != NULL && "store_request callback required.");
    return (alljoyn_keystorelistener) new ajn::KeyStoreListenerCallbackC(callbacks, context);
}

void alljoyn_keystorelistener_destroy(alljoyn_keystorelistener listener)
{
    assert(listener != NULL && "listener parameter must not be NULL");
    delete (ajn::KeyStoreListenerCallbackC*)listener;
}

QStatus alljoyn_keystorelistener_putkeys(alljoyn_keystorelistener listener, alljoyn_keystore keyStore,
                                         const char* source, const char* password)
{
    ajn::KeyStore& ks = *((ajn::KeyStore*)keyStore);
    return ((ajn::KeyStoreListener*)listener)->PutKeys(ks, source, password);
}

QStatus alljoyn_keystorelistener_getkeys(alljoyn_keystorelistener listener, alljoyn_keystore keyStore,
                                         char* sink, size_t sink_sz)
{
    qcc::String sinkStr;
    ajn::KeyStore& ks = *((ajn::KeyStore*)keyStore);
    QStatus ret = ((ajn::KeyStoreListener*)listener)->GetKeys(ks, sinkStr);
    strncpy(sink, sinkStr.c_str(), sink_sz);
    return ret;
}
