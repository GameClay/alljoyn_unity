#ifndef _ALLJOYN_UNITY_SESSION_H
#define _ALLJOYN_UNITY_SESSION_H
/**
 * @file
 * AllJoyn session related data types.
 */

/******************************************************************************
 * Copyright 2011, Qualcomm Innovation Center, Inc.
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

#include <qcc/platform.h>
#include <alljoyn_unity/AjAPI.h>
#include <alljoyn_unity/TransportMask.h>

#ifdef __cplusplus
extern "C" {
#endif

typedef struct _alljoyn_sessionopts_handle*                 alljoyn_sessionopts;

typedef uint16_t alljoyn_sessionport;

/** Invalid SessionPort value used to indicate that BindSessionPort should choose any available port */
const alljoyn_sessionport ALLJOYN_SESSION_PORT_ANY = 0;

/** SessionId uniquely identifies an AllJoyn session instance */
typedef uint32_t alljoyn_sessionid;

#define ALLJOYN_TRAFFIC_TYPE_MESSAGES        0x01   /**< Session carries message traffic */
#define ALLJOYN_TRAFFIC_TYPE_RAW_UNRELIABLE  0x02   /**< Session carries an unreliable (lossy) byte stream */
#define ALLJOYN_TRAFFIC_TYPE_RAW_RELIABLE    0x04   /**< Session carries a reliable byte stream */

#define ALLJOYN_PROXIMITY_ANY       0xFF
#define ALLJOYN_PROXIMITY_PHYSICAL  0x01
#define ALLJOYN_PROXIMITY_NETWORK   0x02

/**
 * Construct a SessionOpts with specific parameters.
 *
 * @param traffic       Type of traffic.
 * @param isMultipoint  true iff session supports multipoint (greater than two endpoints).
 * @param proximity     Proximity constraint bitmask.
 * @param transports    Allowed transport types bitmask.
 *
 */
extern AJ_API alljoyn_sessionopts alljoyn_sessionopts_create(uint8_t traffic, QC_BOOL isMultipoint,
                                                             uint8_t proximity, alljoyn_transportmask transports);

/**
 * Destroy a SessionOpts created with alljoyn_sessionopts_create.
 *
 * @param opts SessionOpts to destroy
 */
extern AJ_API void alljoyn_sessionopts_destroy(alljoyn_sessionopts opts);

/**
 * Accessor for the traffic member of SessionOpts.
 *
 * @param opts SessionOpts
 *
 * @return Traffic type specified by the specified SessionOpts.
 */
extern AJ_API uint8_t alljoyn_sessionopts_traffic(const alljoyn_sessionopts opts);

/**
 * Accessor for the isMultipoint member of SessionOpts.
 *
 * @param opts SessionOpts
 *
 * @return Multipoint value specified by the specified SessionOpts.
 */
extern AJ_API QC_BOOL alljoyn_sessionopts_multipoint(const alljoyn_sessionopts opts);

/**
 * Accessor for the proximity member of SessionOpts.
 *
 * @param opts SessionOpts
 *
 * @return Proximity specified by the specified SessionOpts.
 */
extern AJ_API uint8_t alljoyn_sessionopts_proximity(const alljoyn_sessionopts opts);

/**
 * Accessor for the transports member of SessionOpts.
 *
 * @param opts SessionOpts
 *
 * @return Transports allowed by the specified SessionOpts.
 */
extern AJ_API alljoyn_transportmask alljoyn_sessionopts_transports(const alljoyn_sessionopts opts);

/**
 * Determine whether one SessionOpts is compatible with the SessionOpts offered by other
 *
 * @param one    Options to be compared against other.
 * @param other  Options to be compared against one.
 * @return QC_TRUE iff this SessionOpts can use the option set offered by other.
 */
extern AJ_API QC_BOOL alljoyn_sessionopts_iscompatible(const alljoyn_sessionopts one, const alljoyn_sessionopts other);

/**
 * Compare two SessionOpts.
 *
 * @param one    Options to be compared against other.
 * @param other  Options to be compared against one.
 * @return 0 if the SessionOpts are equal, 1 if one > other, -1 if one < other.
 * @see ajn::SessionOpts::operator<
 * @see ajn::SessionOpts::operator==
 */
extern AJ_API int32_t alljoyn_sessionopts_cmp(const alljoyn_sessionopts one, const alljoyn_sessionopts other);

#ifdef __cplusplus
} /* extern "C" */
#endif

#endif
