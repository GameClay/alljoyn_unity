/**
 * @file
 *
 * This file implements the _Message class
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

#include <assert.h>
#include <ctype.h>

#include <alljoyn/Message.h>
#include <alljoyn/BusAttachment.h>

#define QCC_MODULE "ALLJOYN"

struct _alljoyn_message_handle {
    _alljoyn_message_handle(ajn::BusAttachment& bus) : msg(bus) { }
    _alljoyn_message_handle(const ajn::_Message& other) : msg(other) { }

    ajn::Message msg;
};

alljoyn_message alljoyn_message_create(alljoyn_busattachment bus)
{
    return new struct _alljoyn_message_handle (*((ajn::BusAttachment*)bus));
}

void alljoyn_message_destroy(alljoyn_message msg)
{
    delete msg;
}

const alljoyn_msgargs alljoyn_message_getarg(alljoyn_message msg, size_t argN)
{
    return (alljoyn_msgargs)msg->msg->GetArg(argN);
}
