/**
 * @file
 * BusListener is an abstract base class (interface) implemented by users of the
 * AllJoyn API in order to asynchronously receive bus  related event information.
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
#ifndef _ALLJOYN_UNITY_BUSLISTENER_H
#define _ALLJOYN_UNITY_BUSLISTENER_H

#include <alljoyn_unity/AjAPI.h>
#include <alljoyn_unity/TransportMask.h>

#ifdef __cplusplus
extern "C" {
#endif

typedef struct _alljoyn_buslistener_handle*                 alljoyn_buslistener;
#ifndef _ALLJOYN_OPAQUE_BUSATTACHMENT_
#define _ALLJOYN_OPAQUE_BUSATTACHMENT_
typedef struct _alljoyn_busattachment_handle*               alljoyn_busattachment;
#endif

/**
 * Type for the ListenerRegistered callback.
 */
typedef void (*alljoyn_buslistener_listener_registered_ptr)(const void* context, alljoyn_busattachment bus);

/**
 * Type for the ListenerUnregistered callback.
 */
typedef void (*alljoyn_buslistener_listener_unregistered_ptr)(const void* context);

/**
 * Type for the FoundAdvertisedName callback.
 */
typedef void (*alljoyn_buslistener_found_advertised_name_ptr)(const void* context, const char* name, alljoyn_transportmask transport, const char* namePrefix);

/**
 * Type for the LostAdvertisedName callback.
 */
typedef void (*alljoyn_buslistener_lost_advertised_name_ptr)(const void* context, const char* name, alljoyn_transportmask transport, const char* namePrefix);

/**
 * Type for the NameOwnerChanged callback.
 */
typedef void (*alljoyn_buslistener_name_owner_changed_ptr)(const void* context, const char* busName, const char* previousOwner, const char* newOwner);


/**
 * Type for the BusStopping callback.
 */
typedef void (*alljoyn_buslistener_bus_stopping_ptr)(const void* context);

/**
 * Type for the BusDisconnected callback.
 */
typedef void (*alljoyn_buslistener_bus_disconnected_ptr)(const void* context);

/**
 * Struct containing callbacks used for creation of an alljoyn_buslistener.
 */
typedef struct {
    alljoyn_buslistener_listener_registered_ptr listener_registered;
    alljoyn_buslistener_listener_unregistered_ptr listener_unregistered;
    alljoyn_buslistener_found_advertised_name_ptr found_advertised_name;
    alljoyn_buslistener_lost_advertised_name_ptr lost_advertised_name;
    alljoyn_buslistener_name_owner_changed_ptr name_owner_changed;
    alljoyn_buslistener_bus_stopping_ptr bus_stopping;
    alljoyn_buslistener_bus_disconnected_ptr bus_disconnected;
} alljoyn_buslistener_callbacks;

/**
 * Create a BusListener which will trigger the provided callbacks, passing along the provided context.
 *
 * @param callbacks Callbacks to trigger for associated events.
 * @param context   Context to pass to callback functions
 *
 * @return Handle to newly allocated BusListener.
 */
extern AJ_API alljoyn_buslistener alljoyn_buslistener_create(const alljoyn_buslistener_callbacks* callbacks, const void* context);

/**
 * Destroy a BusListener.
 *
 * @param listener BusListener to destroy.
 */
extern AJ_API void alljoyn_buslistener_destroy(alljoyn_buslistener listener);

#ifdef __cplusplus
} /* extern "C" */
#endif

#endif
