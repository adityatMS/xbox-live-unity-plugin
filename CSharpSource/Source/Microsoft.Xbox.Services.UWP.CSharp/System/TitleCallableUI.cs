// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Xbox.Services.System
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Diagnostics;
    using global::System.Runtime.InteropServices;
    using global::System.Threading;
    using global::System.Threading.Tasks;
    using Microsoft.Xbox.Services;

    public class TitleCallableUI
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct CheckGamingPrivilegeResult
        {
            [MarshalAsAttribute(UnmanagedType.U1)]
            public bool hasPrivilege;

            public XboxLiveResult xblResult;
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void ShowProfileCardUICompletionRoutine(XboxLiveResult result, IntPtr completionRoutineContext);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void CheckGamingPrivilegeCompletionRoutine(CheckGamingPrivilegeResult result, IntPtr completionRoutineContext);

        private delegate int TCUIShowProfileCardUI(IntPtr targetXboxUserId, ShowProfileCardUICompletionRoutine completionRoutine, IntPtr completionRoutineContext);
        private delegate int TCUIShowProfileCardUIForUser(IntPtr targetXboxUserId, IntPtr user, ShowProfileCardUICompletionRoutine completionRoutine, IntPtr completionRoutineContext);

        private delegate int TCUICheckGamingPrivilegeSilently(GamingPrivilege privilege, CheckGamingPrivilegeCompletionRoutine completionRoutine, IntPtr completionRoutineContext);
        private delegate int TCUICheckGamingPrivilegeSilentlyForUser(GamingPrivilege privilege, CheckGamingPrivilegeCompletionRoutine completionRoutine, IntPtr completionRoutineContext);

        private delegate int TCUICheckGamingPrivilegeWithUI(GamingPrivilege privilege, IntPtr friendlyMessage, CheckGamingPrivilegeCompletionRoutine completionRoutine, IntPtr completionRoutineContext);
        private delegate int TCUICheckGamingPrivilegeWithUIForUser(GamingPrivilege privilege, IntPtr friendlyMessage, CheckGamingPrivilegeCompletionRoutine completionRoutine, IntPtr completionRoutineContext);

        private static TaskCompletionSource<bool> showProfileCardUITCS;
        private static TaskCompletionSource<bool> checkPrivilegeSilentlyTCS;
        private static TaskCompletionSource<bool> checkPrivilegeWithUITCS;

        /// <summary>
        /// Shows UI displaying the profile card for a specified user.
        /// </summary>
        /// <param name="targetXboxUserId">The Xbox User ID to show information about.</param>
        /// <returns>
        /// An interface for tracking the progress of the asynchronous call.
        /// The operation completes when the UI is closed.
        /// </returns>
        public static Task ShowProfileCardUIAsync(string targetXboxUserId)
        {
            showProfileCardUITCS = new TaskCompletionSource<bool>();

            Task.Run(() =>
            {
                XboxLive.Instance.Invoke<int, TCUIShowProfileCardUI>(
                    Marshal.StringToHGlobalUni(targetXboxUserId),
                    (ShowProfileCardUICompletionRoutine)ShowProfileCardUIComplete,
                    IntPtr.Zero
                    );
            });

            return showProfileCardUITCS.Task;
        }

        /// <summary>
        /// Shows UI displaying the profile card for a specified user.
        /// </summary>
        /// <param name="targetXboxUserId">The Xbox User ID to show information about.</param>
        /// <param name="user">System user that identifies the user to show the UI on behalf of</param>
        /// <returns>
        /// An interface for tracking the progress of the asynchronous call.
        /// The operation completes when the UI is closed.
        /// </returns>
        public static Task ShowProfileCardUIForUserAsync(string targetXboxUserId, Windows.System.User user)
        {
            showProfileCardUITCS = new TaskCompletionSource<bool>();

            Task.Run(() =>
            {
                XboxLive.Instance.Invoke<int, TCUIShowProfileCardUIForUser>(
                    Marshal.StringToHGlobalUni(targetXboxUserId),
                    user,
                    (ShowProfileCardUICompletionRoutine)ShowProfileCardUIComplete,
                    IntPtr.Zero
                    );
            });

            return showProfileCardUITCS.Task;
        }

        /// <summary>
        /// Checks if the current user has a specific privilege
        /// </summary>
        /// <param name="privilege">The privilege to check.</param>
        /// <returns>
        /// A boolean which is true if the current user has the privilege.
        /// </returns>
        public static bool CheckGamingPrivilegeSilently(GamingPrivilege privilege)
        {
            checkPrivilegeSilentlyTCS = new TaskCompletionSource<bool>();

            Task.Run(() =>
            {
                XboxLive.Instance.Invoke<int, TCUICheckGamingPrivilegeSilently>(
                    privilege,
                    (CheckGamingPrivilegeCompletionRoutine)CheckGamingPrivilegeSilentlyComplete,
                    IntPtr.Zero
                    );
            });

            checkPrivilegeSilentlyTCS.Task.Wait();
            return checkPrivilegeSilentlyTCS.Task.Result;
        }

        /// <summary>
        /// Checks if the current user has a specific privilege
        /// </summary>
        /// <param name="privilege">The privilege to check.</param>
        /// <param name="user">System user that identifies the user to show the UI on behalf of</param>
        /// <returns>
        /// A boolean which is true if the current user has the privilege.
        /// </returns>
        public static bool CheckGamingPrivilegeSilentlyForUser(GamingPrivilege privilege, Windows.System.User user)
        {
            checkPrivilegeSilentlyTCS = new TaskCompletionSource<bool>();

            Task.Run(() =>
            {
                XboxLive.Instance.Invoke<int, TCUICheckGamingPrivilegeSilently>(
                    privilege,
                    user,
                    (CheckGamingPrivilegeCompletionRoutine)CheckGamingPrivilegeSilentlyComplete,
                    IntPtr.Zero
                    );
            });

            checkPrivilegeSilentlyTCS.Task.Wait();
            return checkPrivilegeSilentlyTCS.Task.Result;
        }

        /// <summary>
        /// Checks if the current user has a specific privilege and if it doesn't, it shows UI 
        /// </summary>
        /// <param name="privilege">The privilege to check.</param>
        /// <param name="friendlyMessage">Text to display in addition to the stock text about the privilege</param>
        /// <returns>
        /// An interface for tracking the progress of the asynchronous call.
        /// The operation completes when the UI is closed.
        /// A boolean which is true if the current user has the privilege.
        /// </returns>
        public static Task<bool> CheckGamingPrivilegeWithUI(GamingPrivilege privilege, string friendlyMessage)
        {
            checkPrivilegeWithUITCS = new TaskCompletionSource<bool>();

            Task.Run(() =>
            {
                XboxLive.Instance.Invoke<int, TCUICheckGamingPrivilegeWithUI>(
                    privilege,
                    Marshal.StringToHGlobalUni(friendlyMessage),
                    (CheckGamingPrivilegeCompletionRoutine)CheckGamingPrivilegeWithUIComplete,
                    IntPtr.Zero
                    );
            });

            return checkPrivilegeWithUITCS.Task;
        }

        /// <summary>
        /// Checks if the current user has a specific privilege and if it doesn't, it shows UI 
        /// </summary>
        /// <param name="privilege">The privilege to check.</param>
        /// <param name="friendlyMessage">Text to display in addition to the stock text about the privilege</param>
        /// <param name="user">System user that identifies the user to show the UI on behalf of</param>
        /// <returns>
        /// An interface for tracking the progress of the asynchronous call.
        /// The operation completes when the UI is closed.
        /// A boolean which is true if the current user has the privilege.
        /// </returns>
        public static Task<bool> CheckGamingPrivilegeWithUIForUser(GamingPrivilege privilege, string friendlyMessage, Windows.System.User user)
        {
            checkPrivilegeWithUITCS = new TaskCompletionSource<bool>();

            Task.Run(() =>
            {
                XboxLive.Instance.Invoke<int, TCUICheckGamingPrivilegeWithUI>(
                    privilege,
                    Marshal.StringToHGlobalUni(friendlyMessage),
                    user,
                    (CheckGamingPrivilegeCompletionRoutine)CheckGamingPrivilegeWithUIComplete,
                    IntPtr.Zero
                    );
            });

            return checkPrivilegeWithUITCS.Task;
        }

        private static void ShowProfileCardUIComplete(XboxLiveResult result, IntPtr completionRoutineContext)
        {
            showProfileCardUITCS.SetResult(true);
        }

        private static void CheckGamingPrivilegeSilentlyComplete(CheckGamingPrivilegeResult result, IntPtr completionRoutineContext)
        {
            checkPrivilegeSilentlyTCS.SetResult(result.hasPrivilege);
        }

        private static void CheckGamingPrivilegeWithUIComplete(CheckGamingPrivilegeResult result, IntPtr completionRoutineContext)
        {
            checkPrivilegeWithUITCS.SetResult(result.hasPrivilege);
        }
    }
}