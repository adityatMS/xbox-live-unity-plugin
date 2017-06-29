// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pch.h"
#include "../System/singleton.h"
#include "../System/asyncop.h"

using namespace xbox::services;
using namespace xbox::services::system;

struct xbl_args_tcui_show_profile_card_ui : public xbl_args
{
    PCSTR_T targetXboxUserId;
    IInspectable* user;
    XboxLiveResult result;
};

struct xbl_args_tcui_check_gaming_privilege : public xbl_args
{
    GAMING_PRIVILEGE privilege;
    PCSTR_T friendlyMessage;
    IInspectable* user;
    TCUICheckGamingPrivilegeResult result;
};

void 
show_profile_card_ui_execute_routine(_In_ const std::shared_ptr<xsapi_async_info>& info)
{
    auto args = std::dynamic_pointer_cast<xbl_args_tcui_show_profile_card_ui>(info->args);
    xbox_live_result<void> result;
    if (args->user != nullptr)
    {
        const Microsoft::WRL::ComPtr<IInspectable> spUserInspectable = args->user;
        Windows::System::User^ systemUser = reinterpret_cast<Windows::System::User^>(spUserInspectable.Get());
        result = title_callable_ui::show_profile_card_ui(string_t(args->targetXboxUserId), systemUser).get();
    }
    else
    {
        result = title_callable_ui::show_profile_card_ui(string_t(args->targetXboxUserId)).get();
    }

    XboxLiveResult xboxLiveResult;
    xboxLiveResult.errorCode = result.err().value();
    std::wstring errMessage = std::wstring(result.err_message().begin(), result.err_message().end());
    xboxLiveResult.errorMessage = errMessage.c_str();

    if (info->completionRoutine != nullptr)
    {
        TCUIShowProfileCardUICompletionRoutine callbackFn = static_cast<TCUIShowProfileCardUICompletionRoutine>(info->completionRoutine);
        callbackFn(xboxLiveResult, info->completionRoutineContext);
    }
}

XSAPI_DLLEXPORT void XSAPI_CALL
TCUIShowProfileCardUI(
    _In_ PCSTR_T targetXboxUserId,
    _In_ TCUIShowProfileCardUICompletionRoutine completionRoutine,
    _In_opt_ void* completionRoutineContext
    )
{
    TCUIShowProfileCardUIForUser(targetXboxUserId, nullptr, completionRoutine, completionRoutineContext);
}

XSAPI_DLLEXPORT void XSAPI_CALL
TCUIShowProfileCardUIForUser(
    _In_ PCSTR_T targetXboxUserId,
    _In_ IInspectable* user,
    _In_ TCUIShowProfileCardUICompletionRoutine completionRoutine,
    _In_opt_ void* completionRoutineContext
    )
{
    std::shared_ptr<xsapi_async_info> info = std::make_shared<xsapi_async_info>();
    info->executeRoutine = (xbl_async_op_execute_routine)show_profile_card_ui_execute_routine;
    info->resultRoutine = (xbl_async_op_result_routine)xbl_asyncop_write_result<XboxLiveResult, xbl_args_tcui_show_profile_card_ui>;
    auto tcuiArgs = std::make_shared<xbl_args_tcui_show_profile_card_ui>();
    tcuiArgs->targetXboxUserId = targetXboxUserId;
    tcuiArgs->user = user;
    info->args = tcuiArgs;

    xbl_asyncop_set_info_in_new_handle(info, completionRoutineContext, static_cast<void*>(completionRoutine));
}

void
check_gaming_privilege_silently_execute_routine(_In_ const std::shared_ptr<xsapi_async_info>& info)
{
    auto args = std::dynamic_pointer_cast<xbl_args_tcui_check_gaming_privilege>(info->args);
    xbox_live_result<bool> result;
    if (args->user != nullptr)
    {
        const Microsoft::WRL::ComPtr<IInspectable> spUserInspectable = args->user;
        Windows::System::User^ systemUser = reinterpret_cast<Windows::System::User^>(spUserInspectable.Get());
        result = title_callable_ui::check_gaming_privilege_silently((gaming_privilege)args->privilege, systemUser);
    }
    else
    {
        result = title_callable_ui::check_gaming_privilege_silently((gaming_privilege)args->privilege);
    }

    TCUICheckGamingPrivilegeResult checkGamingPrivilegeResult;
    XboxLiveResult xboxLiveResult;
    xboxLiveResult.errorCode = result.err().value();
    std::wstring errMessage = std::wstring(result.err_message().begin(), result.err_message().end());
    xboxLiveResult.errorMessage = errMessage.c_str();

    checkGamingPrivilegeResult.result = xboxLiveResult;
    checkGamingPrivilegeResult.hasPrivilege = result.payload();

    if (info->completionRoutine != nullptr)
    {
        TCUICheckGamingPrivilegeCompletionRoutine callbackFn = static_cast<TCUICheckGamingPrivilegeCompletionRoutine>(info->completionRoutine);
        callbackFn(checkGamingPrivilegeResult, info->completionRoutineContext);
    }
}

XSAPI_DLLEXPORT void XSAPI_CALL
TCUICheckGamingPrivilegeSilently(
    _In_ GAMING_PRIVILEGE privilege,
    _In_ TCUICheckGamingPrivilegeCompletionRoutine completionRoutine,
    _In_opt_ void* completionRoutineContext
    )
{
    TCUICheckGamingPrivilegeSilentlyForUser(privilege, nullptr, completionRoutine, completionRoutineContext);
}

XSAPI_DLLEXPORT void XSAPI_CALL
TCUICheckGamingPrivilegeSilentlyForUser(
    _In_ GAMING_PRIVILEGE privilege,
    _In_ IInspectable* user,
    _In_ TCUICheckGamingPrivilegeCompletionRoutine completionRoutine,
    _In_opt_ void* completionRoutineContext
    )
{
    std::shared_ptr<xsapi_async_info> info = std::make_shared<xsapi_async_info>();
    info->executeRoutine = (xbl_async_op_execute_routine)check_gaming_privilege_silently_execute_routine;
    info->resultRoutine = (xbl_async_op_result_routine)xbl_asyncop_write_result<TCUICheckGamingPrivilegeResult, xbl_args_tcui_check_gaming_privilege>;
    auto tcuiArgs = std::make_shared<xbl_args_tcui_check_gaming_privilege>();
    tcuiArgs->privilege = privilege;
    tcuiArgs->user = user;
    info->args = tcuiArgs;

    xbl_asyncop_set_info_in_new_handle(info, completionRoutineContext, static_cast<void*>(completionRoutine));
}

void
check_gaming_privilege_with_ui_execute_routine(_In_ const std::shared_ptr<xsapi_async_info>& info)
{
    auto args = std::dynamic_pointer_cast<xbl_args_tcui_check_gaming_privilege>(info->args);

    xbox_live_result<bool> result;
    if (args->user != nullptr)
    {
        const Microsoft::WRL::ComPtr<IInspectable> spUserInspectable = args->user;
        Windows::System::User^ systemUser = reinterpret_cast<Windows::System::User^>(spUserInspectable.Get());
        result = title_callable_ui::check_gaming_privilege_with_ui(
            (gaming_privilege)args->privilege,
            string_t(args->friendlyMessage),
            systemUser
            ).get();
    }
    else
    {
        result = title_callable_ui::check_gaming_privilege_with_ui(
            (gaming_privilege)args->privilege,
            string_t(args->friendlyMessage)
            ).get();
    }

    TCUICheckGamingPrivilegeResult checkGamingPrivilegeResult;
    XboxLiveResult xboxLiveResult;
    xboxLiveResult.errorCode = result.err().value();
    std::wstring errMessage = std::wstring(result.err_message().begin(), result.err_message().end());
    xboxLiveResult.errorMessage = errMessage.c_str();

    checkGamingPrivilegeResult.result = xboxLiveResult;
    checkGamingPrivilegeResult.hasPrivilege = result.payload();

    if (info->completionRoutine != nullptr)
    {
        TCUICheckGamingPrivilegeCompletionRoutine callbackFn = static_cast<TCUICheckGamingPrivilegeCompletionRoutine>(info->completionRoutine);
        callbackFn(checkGamingPrivilegeResult, info->completionRoutineContext);
    }
}

XSAPI_DLLEXPORT void XSAPI_CALL
TCUICheckGamingPrivilegeWithUI(
    _In_ GAMING_PRIVILEGE privilege,
    _In_ PCSTR_T friendlyMessage,
    _In_ TCUICheckGamingPrivilegeCompletionRoutine completionRoutine,
    _In_opt_ void* completionRoutineContext
    )
{
    TCUICheckGamingPrivilegeWithUIForUser(privilege, friendlyMessage, nullptr, completionRoutine, completionRoutineContext);
}

XSAPI_DLLEXPORT void XSAPI_CALL
TCUICheckGamingPrivilegeWithUIForUser(
    _In_ GAMING_PRIVILEGE privilege,
    _In_ PCSTR_T friendlyMessage,
    _In_ IInspectable* user,
    _In_ TCUICheckGamingPrivilegeCompletionRoutine completionRoutine,
    _In_opt_ void* completionRoutineContext
    )
{
    std::shared_ptr<xsapi_async_info> info = std::make_shared<xsapi_async_info>();
    info->executeRoutine = (xbl_async_op_execute_routine)check_gaming_privilege_with_ui_execute_routine;
    info->resultRoutine = (xbl_async_op_result_routine)xbl_asyncop_write_result<TCUICheckGamingPrivilegeResult, xbl_args_tcui_check_gaming_privilege>;
    auto tcuiArgs = std::make_shared<xbl_args_tcui_check_gaming_privilege>();
    tcuiArgs->privilege = privilege;
    tcuiArgs->friendlyMessage = friendlyMessage;
    tcuiArgs->user = user;
    info->args = tcuiArgs;

    xbl_asyncop_set_info_in_new_handle(info, completionRoutineContext, static_cast<void*>(completionRoutine));
}

