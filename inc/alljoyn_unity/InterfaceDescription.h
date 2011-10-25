#ifndef _ALLJOYN_UNITY_INTERFACEDESCRIPTION_H
#define _ALLJOYN_UNITY_INTERFACEDESCRIPTION_H
/**
 * @file
 * This file defines types for statically describing a message bus interface
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
#include <alljoyn_unity/Message.h>
#include <Status.h>

#ifdef __cplusplus
extern "C" {
#endif

typedef struct _alljoyn_interfacedescription_handle*        alljoyn_interfacedescription;

/** @name Access type */
// @{
static const uint8_t ALLJOYN_PROP_ACCESS_READ  = 1; /**< Read Access type */
static const uint8_t ALLJOYN_PROP_ACCESS_WRITE = 2; /**< Write Access type */
static const uint8_t ALLJOYN_PROP_ACCESS_RW    = 3; /**< Read-Write Access type */
// @}
/** @name Anotation flags */
// @{
static const uint8_t ALLJOYN_MEMBER_ANNOTATE_NO_REPLY   = 1; /**< No reply annotate flag */
static const uint8_t ALLJOYN_MEMBER_ANNOTATE_DEPRECATED = 2; /**< Deprecated annotate flag */
// @}

typedef struct {
    alljoyn_interfacedescription iface;         /**< Interface that this member belongs to */
    alljoyn_messagetype memberType;             /**< %Member type */
    const char* name;                           /**< %Member name */
    const char* signature;                      /**< Method call IN arguments (NULL for signals) */
    const char* returnSignature;                /**< Signal or method call OUT arguments */
    const char* argNames;                       /**< Comma separated list of argument names - can be NULL */
    uint8_t annotation;                         /**< Exclusive OR of flags MEMBER_ANNOTATE_NO_REPLY and MEMBER_ANNOTATE_DEPRECATED */

    const void* internal_member;                /**< For internal use only */
} alljoyn_interfacedescription_member;

typedef struct {
    const char* name;               /**< %Property name */
    const char* signature;          /**< %Property type */
    uint8_t access;                 /**< Access is #ALLJOYN_PROP_ACCESS_READ, #ALLJOYN_PROP_ACCESS_WRITE, or #ALLJOYN_PROP_ACCESS_RW */

    const void* internal_property;  /**< For internal use only */
} alljoyn_interfacedescription_property;

/**
 * Activate this interface. An interface must be activated before it can be used. Activating an
 * interface locks the interface so that is can no longer be modified.
 *
 * @param iface InterfaceDescription to activate.
 */
extern AJ_API void alljoyn_interfacedescription_activate(alljoyn_interfacedescription iface);

/**
 * Lookup a member description by name
 *
 * @param       iface   Interface on which to lookup the member
 * @param       name    Name of the member to lookup
 * @param[out]  member  The description of the member
 *
 * @return QC_FALSE if member does not exist, QC_TRUE otherwise.
 */
extern AJ_API QC_BOOL alljoyn_interfacedescription_getmember(const alljoyn_interfacedescription iface, const char* name,
                                                             alljoyn_interfacedescription_member* member);

/**
 * Add a member to the interface.
 *
 * @param iface       Interface on which to add the member.
 * @param type        Message type.
 * @param name        Name of member.
 * @param inputSig    Signature of input parameters or NULL for none.
 * @param outSig      Signature of output parameters or NULL for none.
 * @param argNames    Comma separated list of input and then output arg names used in annotation XML.
 * @param annotation  Annotation flags.
 *
 * @return
 *      - #ER_OK if successful
 *      - #ER_BUS_MEMBER_ALREADY_EXISTS if member already exists
 */
extern AJ_API QStatus alljoyn_interfacedescription_addmember(alljoyn_interfacedescription iface, alljoyn_messagetype type,
                                                             const char* name, const char* inputSig, const char* outSig,
                                                             const char* argNames, uint8_t annotation);

/**
 * Get all the members.
 *
 * @param iface       The interface from which to get all members.
 * @param members     A pointer to a Member array to receive the members. Can be NULL in
 *                    which case no members are returned and the return value gives the number
 *                    of members available.
 * @param numMembers  The size of the Member array. If this value is smaller than the total
 *                    number of members only numMembers will be returned.
 *
 * @return  The number of members returned or the total number of members if members is NULL.
 */
extern AJ_API size_t alljoyn_interfacedescription_getmembers(const alljoyn_interfacedescription iface,
                                                             alljoyn_interfacedescription_member* members,
                                                             size_t numMembers);

/**
 * Check for existence of a member. Optionally check the signature also.
 * @remark
 * if the a signature is not provided this method will only check to see if
 * a member with the given @c name exists.  If a signature is provided a
 * member with the given @c name and @c signature must exist for this to return true.
 *
 * @param iface      Interface to query for a member.
 * @param name       Name of the member to lookup
 * @param inSig      Input parameter signature of the member to lookup
 * @param outSig     Output parameter signature of the member to lookup (leave NULL for signals)
 * @return QC_TRUE if the member name exists.
 */
extern AJ_API QC_BOOL alljoyn_interfacedescription_hasmember(alljoyn_interfacedescription iface,
                                                             const char* name, const char* inSig,
                                                             const char* outSig);

/**
 * Lookup a property description by name
 *
 * @param iface     Interface to query for a property.
 * @param name      Name of the property to lookup
 * @param property  The description of the property
 * @return QC_TRUE if the property was found, QC_FALSE otherwise
 */
extern AJ_API QC_BOOL alljoyn_interfacedescription_getproperty(const alljoyn_interfacedescription iface, const char* name,
                                                               alljoyn_interfacedescription_property* property);

/**
 * Get all the properties.
 *
 * @param iface     Interface to query for properties.
 * @param props     A pointer to a Property array to receive the properties. Can be NULL in
 *                  which case no properties are returned and the return value gives the number
 *                  of properties available.
 * @param numProps  The size of the Property array. If this value is smaller than the total
 *                  number of properties only numProperties will be returned.
 *
 *
 * @return  The number of properties returned or the total number of properties if props is NULL.
 */
extern AJ_API size_t alljoyn_interfacedescription_getproperties(const alljoyn_interfacedescription iface,
                                                                alljoyn_interfacedescription_property* props,
                                                                size_t numProps);

/**
 * Add a property to the interface.
 *
 * @param iface      Interface on which to add the property.
 * @param name       Name of property.
 * @param signature  Property type.
 * @param access     #PROP_ACCESS_READ, #PROP_ACCESS_WRITE or #PROP_ACCESS_RW
 * @return
 *      - #ER_OK if successful.
 *      - #ER_BUS_PROPERTY_ALREADY_EXISTS if the property can not be added
 *                                        because it already exists.
 */
extern AJ_API QStatus alljoyn_interfacedescription_addproperty(alljoyn_interfacedescription iface, const char* name,
                                                               const char* signature, uint8_t access);

/**
 * Check for existence of a property.
 *
 * @param iface      Interface to query.
 * @param name       Name of the property to lookup
 * @return true if the property exists.
 */
extern AJ_API QC_BOOL alljoyn_interfacedescription_hasproperty(const alljoyn_interfacedescription iface, const char* name);

/**
 * Check for existence of any properties
 *
 * @param iface      Interface to query.
 * @return  true if interface has any properties.
 */
extern AJ_API QC_BOOL alljoyn_interfacedescription_hasproperties(const alljoyn_interfacedescription iface);

/**
 * Returns the name of the interface
 *
 * @param iface      Interface to query.
 * @return the interface name.
 */
extern AJ_API const char* alljoyn_interfacedescription_getname(const alljoyn_interfacedescription iface);

/**
 * Indicates if this interface is secure. Secure interfaces require end-to-end authentication.
 * The arguments for methods calls made to secure interfaces and signals emitted by secure
 * interfaces are encrypted.
 * @param iface      Interface to query.
 * @return true if the interface is secure.
 */
extern AJ_API QC_BOOL alljoyn_interfacedescription_issecure(const alljoyn_interfacedescription iface);

/**
 * Equality operation.
 *
 * @param one   Interface to compare to other
 * @param other Interface to compare to one
 *
 * @return QC_TRUE if one == other
 */
extern AJ_API QC_BOOL alljoyn_interfacedescription_eql(const alljoyn_interfacedescription one,
                                                       const alljoyn_interfacedescription other);
#if 0
/* TODO? */
qcc::String Introspect(size_t indent = 0) const;
#endif

#ifdef __cplusplus
} /* extern "C" */
#endif

#undef QCC_MODULE
#endif
