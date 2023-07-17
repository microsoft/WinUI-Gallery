//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AppUIBasics.Helper
{
    internal class NativeHelper
    {
        public const int ERROR_SUCCESS = 0;
        public const int ERROR_INSUFFICIENT_BUFFER = 122;
        public const int APPMODEL_ERROR_NO_PACKAGE = 15700;

        [DllImport("api-ms-win-appmodel-runtime-l1-1-1", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.U4)]
        internal static extern uint GetCurrentPackageId(ref int pBufferLength, out byte pBuffer);
    }
}
