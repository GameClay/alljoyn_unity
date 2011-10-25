/**
 * @file
 * Class for encapsulating Session option information.
 */

/******************************************************************************
 * Copyright 2010-2011, Qualcomm Innovation Center, Inc.
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

#include <alljoyn/MsgArg.h>
#include <alljoyn/Session.h>
#include <alljoyn_unity/MsgArg.h>
#include <alljoyn_unity/Session.h>

#define QCC_MODULE "ALLJOYN"

struct _alljoyn_sessionopts_handle {
    /* Empty by design, this is just to allow the type restrictions to save coders from themselves */
};

alljoyn_sessionopts alljoyn_sessionopts_create(uint8_t traffic, QC_BOOL isMultipoint,
                                               uint8_t proximity, alljoyn_transportmask transports)
{
    return (alljoyn_sessionopts) new ajn::SessionOpts((ajn::SessionOpts::TrafficType)traffic, isMultipoint == QC_TRUE ? true : false,
                                                      (ajn::SessionOpts::Proximity)proximity, (ajn::TransportMask)transports);
}

void alljoyn_sessionopts_destroy(alljoyn_sessionopts opts)
{
    delete (ajn::SessionOpts*)opts;
}

uint8_t alljoyn_sessionopts_traffic(const alljoyn_sessionopts opts)
{
    return ((const ajn::SessionOpts*)opts)->traffic;
}

QC_BOOL alljoyn_sessionopts_multipoint(const alljoyn_sessionopts opts)
{
    return (((const ajn::SessionOpts*)opts)->isMultipoint ? QC_TRUE : QC_FALSE);
}

uint8_t alljoyn_sessionopts_proximity(const alljoyn_sessionopts opts)
{
    return ((const ajn::SessionOpts*)opts)->proximity;
}

alljoyn_transportmask alljoyn_sessionopts_transports(const alljoyn_sessionopts opts)
{
    return ((const ajn::SessionOpts*)opts)->transports;
}

QC_BOOL alljoyn_sessionopts_iscompatible(const alljoyn_sessionopts one, const alljoyn_sessionopts other)
{
    return (((const ajn::SessionOpts*)one)->IsCompatible(*((const ajn::SessionOpts*)other)) == true ? QC_TRUE : QC_FALSE);
}

int32_t alljoyn_sessionopts_cmp(const alljoyn_sessionopts one, const alljoyn_sessionopts other)
{
    const ajn::SessionOpts& _one = *((const ajn::SessionOpts*)one);
    const ajn::SessionOpts& _other = *((const ajn::SessionOpts*)other);

    return (_one == _other ? 0 : (_other < _one ? 1 : -1));
}
