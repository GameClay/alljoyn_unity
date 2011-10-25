/**
 * @file
 * @brief Sample implementation of an AllJoyn service in C.
 *
 * This sample will show how to set up an AllJoyn service that will registered with the
 * wellknown name 'org.alljoyn.Bus.method_sample'.  The service will register a method call
 * with the name 'cat'  this method will take two input strings and return a
 * Concatenated version of the two strings.
 *
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
#include <qcc/platform.h>

#include <assert.h>
#include <signal.h>
#include <stdio.h>
#include <string.h>
#include <stdlib.h>

#include <alljoyn/DBusStd.h>
#include <alljoyn_unity/BusAttachment.h>
#include <alljoyn_unity/version.h>
#include <Status.h>

#include <Status.h>

/** Static top level message bus object */
static alljoyn_busattachment g_msgBus = NULL;

/* Static SessionPortListener */
static alljoyn_sessionportlistener s_sessionPortListener = NULL;

/* Static BusListener */
static alljoyn_buslistener g_busListener = NULL;

/*constants*/
static const char* INTERFACE_NAME = "org.alljoyn.Bus.method_sample";
static const char* SERVICE_NAME = "org.alljoyn.Bus.method_sample";
static const char* SERVICE_PATH = "/method_sample";
static const alljoyn_sessionport SERVICE_PORT = 25;

/** Signal handler
 * with out the signal handler the program will exit without stoping the bus
 * when kill signal is received.  (i.e. [Ctrl + c] is pressed) not using this
 * may result in a memory leak if [cont + c] is used to end this program.
 */
static void SigIntHandler(int sig)
{
    if (NULL != g_msgBus) {
        QStatus status = alljoyn_busattachment_stop(g_msgBus, QC_FALSE);
        if (ER_OK != status) {
            printf("BusAttachment::Stop() failed\n");
        }
    }
}

/* ObjectRegistered callback */
void busobject_object_registered(const void* context)
{
    printf("ObjectRegistered has been called\n");
}

/* NameOwnerChanged callback */
void name_owner_changed(const void* context, const char* busName, const char* previousOwner, const char* newOwner)
{
    if (newOwner && (0 == strcmp(busName, SERVICE_NAME))) {
        printf("NameOwnerChanged: name=%s, oldOwner=%s, newOwner=%s\n",
               busName,
               previousOwner ? previousOwner : "<none>",
               newOwner ? newOwner : "<none>");
    }
}

/* AcceptSessionJoiner callback */
QC_BOOL accept_session_joiner(const void* context, alljoyn_sessionport sessionPort,
                              const char* joiner,  const alljoyn_sessionopts opts)
{
    QC_BOOL ret = QC_FALSE;
    if (sessionPort != SERVICE_PORT) {
        printf("Rejecting join attempt on unexpected session port %d\n", sessionPort);
    } else {
        printf("Accepting join session request from %s (opts.proximity=%x, opts.traffic=%x, opts.transports=%x)\n",
               joiner, alljoyn_sessionopts_proximity(opts), alljoyn_sessionopts_traffic(opts), alljoyn_sessionopts_transports(opts));
        ret = QC_TRUE;
    }
    return ret;
}

/* Exposed concatinate method */
void cat_method(alljoyn_busobject bus, const alljoyn_interfacedescription_member* member, alljoyn_message msg)
{
    /* Concatenate the two input strings and reply with the result. */
    char result[256] = { 0 };
    strncat(result, alljoyn_msgargs_as_string(alljoyn_message_getarg(msg, 0), 0), sizeof(result));
    strncat(result, alljoyn_msgargs_as_string(alljoyn_message_getarg(msg, 1), 0), sizeof(result));

    alljoyn_msgargs outArg = alljoyn_msgargs_create(1);
    size_t numArgs = 1;
    alljoyn_msgargs_set(outArg, 0, &numArgs, "s", result);
    QStatus status = alljoyn_busobject_methodreply_args(bus, msg, outArg, 1);
    if (ER_OK != status) {
        printf("Ping: Error sending reply\n");
    }
}

/** Main entry point */
int main(int argc, char** argv, char** envArg)
{
    QStatus status = ER_OK;

    printf("AllJoyn Library version: %s\n", alljoyn_getversion());
    printf("AllJoyn Library build info: %s\n", alljoyn_getbuildinfo());

    /* Install SIGINT handler */
    signal(SIGINT, SigIntHandler);

    const char* connectArgs = getenv("BUS_ADDRESS");
    if (connectArgs == NULL) {
#ifdef _WIN32
        connectArgs = "tcp:addr=127.0.0.1,port=9955";
#else
        connectArgs = "unix:abstract=alljoyn";
#endif
    }

    /* Create message bus */
    g_msgBus = alljoyn_busattachment_create("myApp", QC_TRUE);

    /* Add org.alljoyn.Bus.method_sample interface */
    alljoyn_interfacedescription testIntf = NULL;
    status = alljoyn_busattachment_createinterface(g_msgBus, INTERFACE_NAME, &testIntf, QC_FALSE);
    if (status == ER_OK) {
        alljoyn_interfacedescription_addmember(testIntf, ALLJOYN_MESSAGE_METHOD_CALL, "cat", "ss",  "s", "inStr1,inStr2,outStr", 0);
        alljoyn_interfacedescription_activate(testIntf);
        printf("Interface Created.\n");
    } else {
        printf("Failed to create interface 'org.alljoyn.Bus.method_sample'\n");
    }

    /* Register a bus listener */
    if (ER_OK == status) {
        /* Create a bus listener */
        alljoyn_buslistener_callbacks callbacks = {
            NULL,
            NULL,
            NULL,
            NULL,
            &name_owner_changed,
            NULL,
            NULL
        };
        g_busListener = alljoyn_buslistener_create(&callbacks, NULL);
        alljoyn_busattachment_registerbuslistener(g_msgBus, g_busListener);
    }

    /* Set up bus object */
    alljoyn_busobject_callbacks busObjCbs = {
        NULL,
        NULL,
        busobject_object_registered,
        NULL
    };
    alljoyn_busobject testObj = alljoyn_busobject_create(g_msgBus, SERVICE_PATH, QC_FALSE, &busObjCbs, NULL);
    const alljoyn_interfacedescription exampleIntf = alljoyn_busattachment_getinterface(g_msgBus, INTERFACE_NAME);
    assert(exampleIntf);
    alljoyn_busobject_addinterface(testObj, exampleIntf);

    alljoyn_interfacedescription_member cat_member;
    QC_BOOL foundMember = alljoyn_interfacedescription_getmember(exampleIntf, "cat", &cat_member);
    assert(foundMember == QC_TRUE);

    alljoyn_busobject_methodentry methodEntries[] = {
        { &cat_member, cat_method },
    };
    status = alljoyn_busobject_addmethodhandlers(testObj, methodEntries, sizeof(methodEntries) / sizeof(methodEntries[0]));
    if (ER_OK != status) {
        printf("Failed to register method handlers for BasicSampleObject");
    }

    /* Start the msg bus */
    status = alljoyn_busattachment_start(g_msgBus);
    if (ER_OK == status) {
        printf("BusAttachement started.\n");
        /* Register  local objects and connect to the daemon */
        alljoyn_busattachment_registerbusobject(g_msgBus, testObj);

        /* Create the client-side endpoint */
        status = alljoyn_busattachment_connect(g_msgBus, connectArgs);
        if (ER_OK != status) {
            printf("Failed to connect to \"%s\"\n", connectArgs);
            exit(1);
        } else {
            printf("Connected to '%s'\n", connectArgs);
        }
    } else {
        printf("BusAttachment::Start failed\n");
    }

    /*
     * Advertise this service on the bus
     * There are three steps to advertising this service on the bus
     * 1) Request a well-known name that will be used by the client to discover
     *    this service
     * 2) Create a session
     * 3) Advertise the well-known name
     */
    /* Request name */
    if (ER_OK == status) {
        uint32_t flags = DBUS_NAME_FLAG_REPLACE_EXISTING | DBUS_NAME_FLAG_DO_NOT_QUEUE;
        QStatus status = alljoyn_busattachment_requestname(g_msgBus, SERVICE_NAME, flags);
        if (ER_OK != status) {
            printf("RequestName(%s) failed (status=%s)\n", SERVICE_NAME, QCC_StatusText(status));
        }
    }

    /* Create session port listener */
    alljoyn_sessionportlistener_callbacks spl_cbs = {
        accept_session_joiner,
        NULL
    };
    s_sessionPortListener = alljoyn_sessionportlistener_create(&spl_cbs, NULL);

    /* Create session */
    alljoyn_sessionopts opts = alljoyn_sessionopts_create(ALLJOYN_TRAFFIC_TYPE_MESSAGES, QC_FALSE, ALLJOYN_PROXIMITY_ANY, ALLJOYN_TRANSPORT_ANY);
    if (ER_OK == status) {
        alljoyn_sessionport sp = SERVICE_PORT;
        status = alljoyn_busattachment_bindsessionport(g_msgBus, &sp, opts, s_sessionPortListener);
        if (ER_OK != status) {
            printf("BindSessionPort failed (%s)\n", QCC_StatusText(status));
        }
    }

    /* Advertise name */
    if (ER_OK == status) {
        status = alljoyn_busattachment_advertisename(g_msgBus, SERVICE_NAME, alljoyn_sessionopts_transports(opts));
        if (status != ER_OK) {
            printf("Failed to advertise name %s (%s)\n", SERVICE_NAME, QCC_StatusText(status));
        }
    }

    if (ER_OK == status) {
        /*
         * Wait until bus is stopped
         */
        alljoyn_busattachment_waitstop(g_msgBus);
    }

    /* Deallocate bus */
    if (g_msgBus) {
        alljoyn_busattachment deleteMe = g_msgBus;
        g_msgBus = NULL;
        alljoyn_busattachment_destroy(deleteMe);
    }

    /* Deallocate bus listener */
    if (g_busListener) {
        alljoyn_buslistener_destroy(g_busListener);
    }

    /* Deallocate session port listener */
    if (s_sessionPortListener) {
        alljoyn_sessionportlistener_destroy(s_sessionPortListener);
    }

    return (int) status;
}
