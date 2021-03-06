// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#pragma once

#pragma warning(disable: 4062)

#ifdef _WIN32
    #include <windows.h>

    #ifndef _WIN32_WINNT_WIN10
    #define _WIN32_WINNT_WIN10 0x0A00
    #endif

    #ifndef XDK_API
    #define XDK_API (WINAPI_FAMILY == WINAPI_FAMILY_TV_APP || WINAPI_FAMILY == WINAPI_FAMILY_TV_TITLE) 
    #endif

    #ifndef UWP_API
    #define UWP_API (WINAPI_FAMILY == WINAPI_FAMILY_APP && _WIN32_WINNT >= _WIN32_WINNT_WIN10)
    #endif
#endif //#ifdef _WIN32

#ifndef _WIN32
    #ifdef _In_
    #undef _In_
    #endif
    #define _In_

    #ifdef _Ret_maybenull_
    #undef _Ret_maybenull_
    #endif
    #define _Ret_maybenull_

    #ifdef _Post_writable_byte_size_
    #undef _Post_writable_byte_size_
    #endif
    #define _Post_writable_byte_size_(X)
#endif

#if defined _WIN32
  #ifdef XBL_API_NONE
    #define XBL_API
    #if _MSC_VER >= 1900
        #define XBL_API_DEPRECATED __declspec(deprecated)
    #else
        #define XBL_API_DEPRECATED
    #endif
  #else
    #ifdef XBL_API_EXPORT
      #define XBL_API __declspec(dllexport)
      #define XBL_API_DEPRECATED __declspec(dllexport, deprecated)
    #else
      #define XBL_API __declspec(dllimport)
      #define XBL_API_DEPRECATED __declspec(dllimport, deprecated)
    #endif
  #endif
#else
  #if defined _NOXBL_API || __GNUC__ < 4
     #define XBL_API
     #define XBL_API_DEPRECATED __attribute__ ((deprecated))
  #else
    #define XBL_API __attribute__ ((visibility ("default")))
    #define XBL_API_DEPRECATED __attribute__ ((visibility ("default"), deprecated))
  #endif
#endif

#ifndef _T
    #ifdef _WIN32
        #define _T(x) L ## x
    #else
        #define _T(x) x
    #endif
#endif

#ifndef XSAPI_DLLEXPORT 
    #define XSAPI_DLLEXPORT __declspec(dllexport)
#else
    #define XSAPI_DLLEXPORT __declspec(dllimport)
#endif

#define XBL_CALLING_CONV __cdecl

#if defined(__cplusplus)
extern "C" {
#endif

typedef enum XSAPI_RESULT
{
    XSAPI_OK = 0,
    XSAPI_E_FAIL = -1,
    XSAPI_E_POINTER = -2,
    XSAPI_E_INVALIDARG = -3,
    XSAPI_E_OUTOFMEMORY = -4,
    XSAPI_E_BUFFERTOOSMALL = -5,
    XSAPI_E_NOTINITIALIZED = -6,
    XSAPI_E_FEATURENOTPRESENT = -7
} XSAPI_RESULT;

#ifdef __cplusplus
#define XSAPI_NOEXCEPT noexcept
#else
#define XSAPI_NOEXCEPT
#endif

#if defined(__cplusplus)
} // end extern "C"
#endif // defined(__cplusplus)
