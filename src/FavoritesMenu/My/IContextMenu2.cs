﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using Windows.Win32.Foundation;

namespace FavoritesMenu.My;

// Don't use the IContextMenu2 interface generated by CsWin32 because CsWin32 converts HandleMenuMsg (which originally returns HRESULT)
// to a void method and throws a COMException if result is != 0.
// But this method is called in WndProc and returns != 0 if it doesn't handle the window message. So we create and throw a COMException
// for many window messages, which is slow.
// That's why we use this interface, which returns the HRESULT and that can be checked without the overhead of exceptions.
[Guid("000214F4-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), ComImport()]
[SupportedOSPlatform("windows5.1.2600")]
[global::System.CodeDom.Compiler.GeneratedCode("Microsoft.Windows.CsWin32", "0.3.49-beta+91f5c15987")]
internal interface IContextMenu2
            : Windows.Win32.UI.Shell.IContextMenu
{
    new void QueryContextMenu(Windows.Win32.UI.WindowsAndMessaging.HMENU hmenu, uint indexMenu, uint idCmdFirst, uint idCmdLast, uint uFlags);

    unsafe new void InvokeCommand(Windows.Win32.UI.Shell.CMINVOKECOMMANDINFO* pici);

    unsafe new void GetCommandString(nuint idCmd, uint uType, [Optional] uint* pReserved, Windows.Win32.Foundation.PSTR pszName, uint cchMax);

    /// <summary>Enables client objects of the IContextMenu interface to handle messages associated with owner-drawn menu items.</summary>
    /// <param name="uMsg">
    /// <para>Type: <b>UINT</b> The message to be processed. In the case of some messages, such as WM_INITMENUPOPUP, WM_DRAWITEM, WM_MENUCHAR, or WM_MEASUREITEM, the client object being called may provide owner-drawn menu items.</para>
    /// <para><see href="https://learn.microsoft.com/windows/win32/api/shobjidl_core/nf-shobjidl_core-icontextmenu2-handlemenumsg#parameters">Read more on docs.microsoft.com</see>.</para>
    /// </param>
    /// <param name="wParam">
    /// <para>Type: <b>WPARAM</b> Additional message information. The value of this parameter depends on the value of the <i>uMsg</i> parameter.</para>
    /// <para><see href="https://learn.microsoft.com/windows/win32/api/shobjidl_core/nf-shobjidl_core-icontextmenu2-handlemenumsg#parameters">Read more on docs.microsoft.com</see>.</para>
    /// </param>
    /// <param name="lParam">
    /// <para>Type: <b>LPARAM</b> Additional message information. The value of this parameter depends on the value of the <i>uMsg</i> parameter.</para>
    /// <para><see href="https://learn.microsoft.com/windows/win32/api/shobjidl_core/nf-shobjidl_core-icontextmenu2-handlemenumsg#parameters">Read more on docs.microsoft.com</see>.</para>
    /// </param>
    /// <returns>
    /// <para>Type: <b>HRESULT</b> If this method succeeds, it returns <b>S_OK</b>. Otherwise, it returns an <b>HRESULT</b> error code.</para>
    /// </returns>
    /// <remarks>
    /// <para><b>IContextMenu2::HandleMenuMsg</b> is generally replaced by <a href="https://docs.microsoft.com/windows/desktop/api/shobjidl_core/nf-shobjidl_core-icontextmenu3-handlemenumsg2">HandleMenuMsg2</a>. <b>HandleMenuMsg2</b> is called when <a href="https://docs.microsoft.com/windows/desktop/api/shobjidl_core/nn-shobjidl_core-icontextmenu">IContextMenu</a> determines that <a href="https://docs.microsoft.com/windows/desktop/api/shobjidl_core/nn-shobjidl_core-icontextmenu3">IContextMenu3</a> is supported and receives one of the messages specified in the description of the <i>uMsg</i> parameter. However, in some cases, <b>IContextMenu2::HandleMenuMsg</b> is still called. If <a href="https://docs.microsoft.com/windows/desktop/api/shobjidl_core/nn-shobjidl_core-icontextmenu2">IContextMenu2</a> or <a href="https://docs.microsoft.com/windows/desktop/api/shobjidl_core/nn-shobjidl_core-icontextmenu3">IContextMenu3</a> is needed, the best implementation for new context menus is to implement all their logic in <a href="https://docs.microsoft.com/windows/desktop/api/shobjidl_core/nf-shobjidl_core-icontextmenu3-handlemenumsg2">HandleMenuMsg2</a> and have their <b>IContextMenu2::HandleMenuMsg</b> implementation simply delegate the call to <b>HandleMenuMsg2</b> and pass <b>NULL</b> as the <i>plResult</i> parameter.</para>
    /// <para><div class="alert"><b>Note</b>  If <a href="https://docs.microsoft.com/windows/desktop/api/shobjidl_core/nn-shobjidl_core-icontextmenu3">IContextMenu3</a> is not implemented, there is no guarantee that <a href="https://docs.microsoft.com/windows/desktop/api/shobjidl_core/nn-shobjidl_core-icontextmenu2">IContextMenu2</a> will be called in its place. In some cases, the absence of <b>IContextMenu3</b> is determined and then the process is halted. </div> <div> </div></para>
    /// <para><see href="https://learn.microsoft.com/windows/win32/api/shobjidl_core/nf-shobjidl_core-icontextmenu2-handlemenumsg#">Read more on docs.microsoft.com</see>.</para>
    /// </remarks>
    [PreserveSig]
    HRESULT HandleMenuMsg(int uMsg, IntPtr wParam, IntPtr lParam);
}
