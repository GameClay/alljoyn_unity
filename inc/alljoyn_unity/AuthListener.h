#ifndef _ALLJOYN_UNITY_AUTHLISTENER_H
#define _ALLJOYN_UNITY_AUTHLISTENER_H
/**
 * @file
 * This file defines the AuthListener class that provides the interface between authentication
 * mechansisms and applications.
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
#include <alljoyn_unity/AjAPI.h>
#include <alljoyn_unity/Message.h>

#ifdef __cplusplus
extern "C" {
#endif

typedef struct _alljoyn_authlistener_handle*                alljoyn_authlistener;
typedef struct _alljoyn_credentials_handle*                 alljoyn_credentials;

/**
 * @name Credential indication Bitmasks
 *  Bitmasks used to indicated what type of credentials are being used.
 */
// @{
static const uint16_t ALLJOYN_CRED_PASSWORD     = 0x0001; /**< Bit 0 indicates credentials include a password, pincode, or passphrase */
static const uint16_t ALLJOYN_CRED_USER_NAME    = 0x0002; /**< Bit 1 indicates credentials include a user name */
static const uint16_t ALLJOYN_CRED_CERT_CHAIN   = 0x0004; /**< Bit 2 indicates credentials include a chain of PEM-encoded X509 certificates */
static const uint16_t ALLJOYN_CRED_PRIVATE_KEY  = 0x0008; /**< Bit 3 indicates credentials include a PEM-encoded private key */
static const uint16_t ALLJOYN_CRED_LOGON_ENTRY  = 0x0010; /**< Bit 4 indicates credentials include a logon entry that can be used to logon a remote user */
static const uint16_t ALLJOYN_CRED_EXPIRATION   = 0x0020; /**< Bit 5 indicates credentials include an expiration time */
// @}
/**
 * @name Credential request values
 * These values are only used in a credential request
 */
// @{
static const uint16_t ALLJOYN_CRED_NEW_PASSWORD = 0x1001; /**< Indicates the credential request is for a newly created password */
static const uint16_t ALLJOYN_CRED_ONE_TIME_PWD = 0x2001; /**< Indicates the credential request is for a one time use password */
// @}

/**
 * Type for the RequestCredentials callback.
 */
typedef QC_BOOL (*alljoyn_authlistener_requestcredentials_ptr)(const void* context, const char* authMechanism, const char* peerName, uint16_t authCount,
                                                               const char* userName, uint16_t credMask, alljoyn_credentials credentials);
/**
 * Type for the VerifyCredentials callback.
 */
typedef QC_BOOL (*alljoyn_authlistener_verifycredentials_ptr)(const void* context, const char* authMechanism, const char* peerName,
                                                              const alljoyn_credentials credentials);
/**
 * Type for the SecurityViolation callback.
 */
typedef void (*alljoyn_authlistener_securityviolation_ptr)(const void* context, QStatus status, const alljoyn_message msg);

/**
 * Type for the AuthenticationComplete callback.
 */
typedef void (*alljoyn_authlistener_authenticationcomplete_ptr)(const void* context, const char* authMechanism, const char* peerName, QC_BOOL success);

/**
 * Structure used during alljoyn_authlistener_create to provide callbacks into C.
 */
typedef struct {
    alljoyn_authlistener_requestcredentials_ptr request_credentials;
    alljoyn_authlistener_verifycredentials_ptr verify_credentials;
    alljoyn_authlistener_securityviolation_ptr security_violation;
    alljoyn_authlistener_authenticationcomplete_ptr authentication_complete;
} alljoyn_authlistener_callbacks;

/**
 * Create a AuthListener which will trigger the provided callbacks, passing along the provided context.
 *
 * @param callbacks Callbacks to trigger for associated events.
 * @param context   Context to pass to callback functions
 *
 * @return Handle to newly allocated AuthListener.
 */
extern AJ_API alljoyn_authlistener alljoyn_authlistener_create(const alljoyn_authlistener_callbacks* callbacks, const void* context);

/**
 * Destroy a AuthListener.
 *
 * @param listener AuthListener to destroy.
 */
extern AJ_API void alljoyn_authlistener_destroy(alljoyn_authlistener listener);

/**
 * Create credentials
 *
 * @return Newly created credentials.
 */
extern AJ_API alljoyn_credentials alljoyn_credentials_create();

/**
 * Destroy credentials
 *
 * @param cred Credentials to destroy.
 */
extern AJ_API void alljoyn_credentials_destroy(alljoyn_credentials cred);

/**
 * Tests if one or more credentials are set.
 *
 * @param cred   The credentials to test.
 * @param creds  A logical or of the credential bit values.
 * @return true if the credentials are set.
 */
extern AJ_API QC_BOOL alljoyn_credentials_isset(const alljoyn_credentials cred, uint16_t creds);

/**
 * Sets a requested password, pincode, or passphrase.
 *
 * @param cred The credentials to set.
 * @param pwd  The password to set.
 */
extern AJ_API void alljoyn_credentials_setpassword(alljoyn_credentials cred, const char* pwd);

/**
 * Sets a requested user name.
 *
 * @param cred      The credentials to set.
 * @param userName  The user name to set.
 */
extern AJ_API void alljoyn_credentials_setusername(alljoyn_credentials cred, const char* userName);

/**
 * Sets a requested public key certificate chain. The certificates must be PEM encoded.
 *
 * @param cred       The credentials to set.
 * @param certChain  The certificate chain to set.
 */
extern AJ_API void alljoyn_credentials_setcertchain(alljoyn_credentials cred, const char* certChain);

/**
 * Sets a requested private key. The private key must be PEM encoded and may be encrypted. If
 * the private key is encrypted the passphrase required to decrypt it must also be supplied.
 *
 * @param cred The credentials to set.
 * @param pk   The private key to set.
 */
extern AJ_API void alljoyn_credentials_setprivatekey(alljoyn_credentials cred, const char* pk);

/**
 * Sets a logon entry. For example for the Secure Remote Password protocol in RFC 5054, a
 * logon entry encodes the N,g, s and v parameters. An SRP logon entry string has the form
 * N:g:s:v where N,g,s, and v are ASCII encoded hexadecimal strings and are separated by
 * colons.
 *
 * @param cred        The credentials to set.
 * @param logonEntry  The logon entry to set.
 */
extern AJ_API void alljoyn_credentials_setlogonentry(alljoyn_credentials cred, const char* logonEntry);

/**
 * Sets an expiration time in seconds relative to the current time for the credentials. This value is optional and
 * can be set on any response to a credentials request. After the specified expiration time has elapsed any secret
 * keys based on the provided credentials are invalidated and a new authentication exchange will be required. If an
 * expiration is not set the default expiration time for the requested authentication mechanism is used.
 *
 * @param cred        The credentials to set.
 * @param expiration  The expiration time in seconds.
 */
extern AJ_API void alljoyn_credentials_setexpiration(alljoyn_credentials cred, uint32_t expiration);

/**
 * Gets the password, pincode, or passphrase from this credentials instance.
 *
 * @param cred The credentials to query.
 * @return A password or an empty string.
 */
extern AJ_API const char* alljoyn_credentials_getpassword(const alljoyn_credentials cred);

/**
 * Gets the user name from this credentials instance.
 *
 * @param cred The credentials to query.
 * @return A user name or an empty string.
 */
extern AJ_API const char* alljoyn_credentials_getusername(const alljoyn_credentials cred);

/**
 * Gets the PEM encoded X509 certificate chain from this credentials instance.
 *
 * @param cred The credentials to query.
 * @return An X509 certificate chain or an empty string.
 */
extern AJ_API const char* alljoyn_credentials_getcertchain(const alljoyn_credentials cred);

/**
 * Gets the PEM encode private key from this credentials instance.
 *
 * @param cred The credentials to query.
 * @return An PEM encode private key or an empty string.
 */
extern AJ_API const char* alljoyn_credentials_getprivateKey(const alljoyn_credentials cred);

/**
 * Gets a logon entry.
 *
 * @param cred The credentials to query.
 * @return An encoded logon entry or an empty string.
 */
extern AJ_API const char* alljoyn_credentials_getlogonentry(const alljoyn_credentials cred);

/**
 * Get the expiration time in seconds if it is set.
 *
 * @param cred The credentials to query.
 * @return The expiration or the max 32 bit unsigned value if it was not set.
 */
extern AJ_API uint32_t alljoyn_credentials_getexpiration(const alljoyn_credentials cred);

/**
 * Clear the credentials.
 *
 * @param cred The credentials to clear.
 */
extern AJ_API void alljoyn_credentials_clear(alljoyn_credentials cred);

#ifdef __cplusplus
} /* extern "C" */
#endif

#endif
