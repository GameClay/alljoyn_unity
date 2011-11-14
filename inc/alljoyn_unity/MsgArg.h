#ifndef _ALLJOYN_UNITY_MSGARG_H
#define _ALLJOYN_UNITY_MSGARG_H
/**
 * @file
 * This file defines a class for message bus data types and values
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

#include <qcc/platform.h>
#include <alljoyn_unity/AjAPI.h>
#include <stdarg.h>
#include <Status.h>

#ifdef __cplusplus
extern "C" {
#endif

typedef struct _alljoyn_msgargs_handle*                     alljoyn_msgargs;

/**
 * Create a new message argument array.
 *
 * @param numArgs Number of arguments to create in the array.
 */
extern AJ_API alljoyn_msgargs alljoyn_msgargs_create(size_t numArgs);

/**
 * Destroy a message argument.
 *
 * @param arg The message argument to destroy.
 */
extern AJ_API void alljoyn_msgargs_destroy(alljoyn_msgargs arg);

/**
 * Set an array of MsgArgs by applying the Set() method to each MsgArg in turn.
 *
 * @param args        An array of MsgArgs to set.
 * @param argOffset   Offset from the start of the MsgArg array.
 * @param numArgs     [in,out] On input the number of args to set. On output the number of MsgArgs
 *                    that were set. There must be at least enought MsgArgs to completely
 *                    initialize the signature.
 *                    there should at least enough.
 * @param signature   The signature for MsgArg values
 * @param ...         One or more values to initialize the MsgArg list.
 *
 * @return
 *       - #ER_OK if the MsgArgs were successfully set.
 *       - #ER_BUS_TRUNCATED if the signature was longer than expected.
 *       - Other error status codes indicating a failure.
 */
extern AJ_API QStatus alljoyn_msgargs_set(alljoyn_msgargs args, size_t argOffset, size_t* numArgs, const char* signature, ...);

extern AJ_API uint8_t alljoyn_msgargs_as_uint8(const alljoyn_msgargs args, size_t idx);
extern AJ_API QC_BOOL alljoyn_msgargs_as_bool(const alljoyn_msgargs args, size_t idx);
extern AJ_API int16_t alljoyn_msgargs_as_int16(const alljoyn_msgargs args, size_t idx);
extern AJ_API uint16_t alljoyn_msgargs_as_uint16(const alljoyn_msgargs args, size_t idx);
extern AJ_API int32_t alljoyn_msgargs_as_int32(const alljoyn_msgargs args, size_t idx);
extern AJ_API uint32_t alljoyn_msgargs_as_uint32(const alljoyn_msgargs args, size_t idx);
extern AJ_API int64_t alljoyn_msgargs_as_int64(const alljoyn_msgargs args, size_t idx);
extern AJ_API uint64_t alljoyn_msgargs_as_uint64(const alljoyn_msgargs args, size_t idx);
extern AJ_API double alljoyn_msgargs_as_double(const alljoyn_msgargs args, size_t idx);
extern AJ_API const char* alljoyn_msgargs_as_string(const alljoyn_msgargs args, size_t idx);
extern AJ_API const char* alljoyn_msgargs_as_objpath(const alljoyn_msgargs args, size_t idx);
extern AJ_API alljoyn_msgargs alljoyn_msgargs_as_variant(const alljoyn_msgargs args, size_t idx);
extern AJ_API void alljoyn_msgargs_as_signature(const alljoyn_msgargs args, size_t idx,
                                                uint8_t* out_len, const char** out_sig);
extern AJ_API void alljoyn_msgargs_as_handle(const alljoyn_msgargs args, size_t idx,
                                             void** out_socketFd);
extern AJ_API const alljoyn_msgargs alljoyn_msgargs_as_array(const alljoyn_msgargs args, size_t idx,
                                                             size_t* out_len, const char** out_sig);
extern AJ_API alljoyn_msgargs alljoyn_msgargs_as_struct(const alljoyn_msgargs args, size_t idx,
                                                        size_t* out_numMembers);
extern AJ_API void alljoyn_msgargs_as_dictentry(const alljoyn_msgargs args, size_t idx,
                                                alljoyn_msgargs* out_key, alljoyn_msgargs* out_val);
extern AJ_API void alljoyn_msgargs_as_scalararray(const alljoyn_msgargs args, size_t idx,
                                                  size_t* out_numElements, const void** out_elements);
#ifdef __cplusplus
} /* extern "C" */
#endif

#endif
