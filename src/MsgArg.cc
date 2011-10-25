/**
 * @file
 *
 * This file implements the MsgArg class
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

#include <cstdarg>
#include <assert.h>

#include <alljoyn/Message.h>
#include <alljoyn/MsgArg.h>
#include <alljoyn_unity/Message.h>
#include <alljoyn_unity/MsgArg.h>

#define QCC_MODULE "ALLJOYN"

namespace ajn {
extern QStatus MsgArgUtils_SetV(MsgArg* args, size_t& numArgs, const char* signature, va_list* argp);
}

struct _alljoyn_msgargs_handle {
    /* Empty by design, this is just to allow the type restrictions to save coders from themselves */
};

alljoyn_msgargs alljoyn_msgargs_create(size_t numArgs)
{
    ajn::MsgArg* args = new ajn::MsgArg[numArgs];
    for (size_t i = 0; i < numArgs; i++) {
        args[i].Clear();
    }
    return (alljoyn_msgargs)args;
}

void alljoyn_msgargs_destroy(alljoyn_msgargs arg)
{
    assert(arg != NULL && "NULL argument passed to alljoyn_msgarg_destroy.");
    delete [] (ajn::MsgArg*)arg;
}

QStatus alljoyn_msgargs_set(alljoyn_msgargs args, size_t argOffset, size_t* numArgs, const char* signature, ...)
{
    va_list argp;
    va_start(argp, signature);
    QStatus status = ajn::MsgArgUtils_SetV(((ajn::MsgArg*)args) + argOffset, *numArgs, signature, &argp);
    va_end(argp);
    return status;
}

#define _IMPLEMENT_MSGARG_TYPE_ACCESSOR(rt, nt, mt) \
    rt alljoyn_msgargs_as_ ## nt(const alljoyn_msgargs args, size_t idx) \
    { \
        return ((ajn::MsgArg*)args)[idx].mt; \
    }
#define _IMPLEMENT_MSGARG_TYPE_ACCESSOR_S(t) _IMPLEMENT_MSGARG_TYPE_ACCESSOR(t ## _t, t, v_ ## t)

_IMPLEMENT_MSGARG_TYPE_ACCESSOR_S(int16);
_IMPLEMENT_MSGARG_TYPE_ACCESSOR_S(uint16);
_IMPLEMENT_MSGARG_TYPE_ACCESSOR_S(int32);
_IMPLEMENT_MSGARG_TYPE_ACCESSOR_S(uint32);
_IMPLEMENT_MSGARG_TYPE_ACCESSOR_S(int64);
_IMPLEMENT_MSGARG_TYPE_ACCESSOR_S(uint64);

_IMPLEMENT_MSGARG_TYPE_ACCESSOR(uint8_t, uint8_t, v_byte);
_IMPLEMENT_MSGARG_TYPE_ACCESSOR(QC_BOOL, bool, v_bool);
_IMPLEMENT_MSGARG_TYPE_ACCESSOR(double, double, v_double);

#undef _IMPLEMENT_MSGARG_TYPE_ACCESSOR
#undef _IMPLEMENT_MSGARG_TYPE_ACCESSOR_S

const char* alljoyn_msgargs_as_string(const alljoyn_msgargs args, size_t idx)
{
    return ((ajn::MsgArg*)args)[idx].v_string.str;
}

