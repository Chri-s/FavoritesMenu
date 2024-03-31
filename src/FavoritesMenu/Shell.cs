using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Media;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Shell;
using Windows.Win32.UI.Shell.Common;
using Windows.Win32.UI.WindowsAndMessaging;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;
using System.Drawing;


namespace FavoritesMenu;
internal class Shell
{
    private static readonly Guid IID_IShellFolder = new Guid("000214E6-0000-0000-C000-000000000046");
    private static readonly Guid IID_IContextMenu = new Guid("000214E4-0000-0000-C000-000000000046");
    private static readonly Guid IID_IContextMenu2 = new Guid("000214F4-0000-0000-C000-000000000046");

    private const uint SCRATCH_QCM_FIRST = 0x1;
    private const uint SCRATCH_QCM_LAST = 0x7FFF;

    private const uint CMIC_MASK_PTINVOKE = 0x20000000;

    private const uint CMIC_MASK_SHIFT_DOWN = 0x10000000;
    private const uint CMIC_MASK_CONTROL_DOWN = 0x40000000;

    private static Dictionary<int, ImageSource> imageList = new Dictionary<int, ImageSource>();
    private static Dictionary<int, ImageSource> largeImageList = new Dictionary<int, ImageSource>();

    public static unsafe ImageSource? GetFileIcon(string path)
    {
        SHFILEINFOW shinfo = new SHFILEINFOW();

        nuint hImgList = PInvoke.SHGetFileInfo(path, Windows.Win32.Storage.FileSystem.FILE_FLAGS_AND_ATTRIBUTES.SECURITY_ANONYMOUS, &shinfo, (uint)Marshal.SizeOf(shinfo), SHGFI_FLAGS.SHGFI_ICON | SHGFI_FLAGS.SHGFI_SMALLICON | SHGFI_FLAGS.SHGFI_SYSICONINDEX);
        if (hImgList == 0)
            return null;

        if (imageList.TryGetValue(shinfo.iIcon, out ImageSource? imageSource))
            return imageSource;

        HICON icon = PInvoke.ImageList_GetIcon(new Windows.Win32.UI.Controls.HIMAGELIST((nint)hImgList), shinfo.iIcon, Windows.Win32.UI.Controls.IMAGE_LIST_DRAW_STYLE.ILD_NORMAL);

        imageSource = Imaging.CreateBitmapSourceFromHIcon(icon, new Int32Rect(0, 0, 16, 16), BitmapSizeOptions.FromEmptyOptions());
        imageSource.Freeze();
        PInvoke.DestroyIcon(shinfo.hIcon);

        imageList.Add(shinfo.iIcon, imageSource);

        return imageSource;
    }

    public static unsafe ImageSource? GetLargeFileIcon(string path)
    {
        SHFILEINFOW shinfo = new SHFILEINFOW();

        nuint hImgList = PInvoke.SHGetFileInfo(path, Windows.Win32.Storage.FileSystem.FILE_FLAGS_AND_ATTRIBUTES.SECURITY_ANONYMOUS, &shinfo, (uint)Marshal.SizeOf(shinfo), SHGFI_FLAGS.SHGFI_ICON | SHGFI_FLAGS.SHGFI_LARGEICON | SHGFI_FLAGS.SHGFI_SYSICONINDEX);
        if (hImgList == 0)
            return null;

        if (largeImageList.TryGetValue(shinfo.iIcon, out ImageSource? imageSource))
            return imageSource;

        HICON icon = PInvoke.ImageList_GetIcon(new Windows.Win32.UI.Controls.HIMAGELIST((nint)hImgList), shinfo.iIcon, Windows.Win32.UI.Controls.IMAGE_LIST_DRAW_STYLE.ILD_NORMAL);

        imageSource = Imaging.CreateBitmapSourceFromHIcon(icon, new Int32Rect(0, 0, 32, 32), BitmapSizeOptions.FromEmptyOptions());
        imageSource.Freeze();
        PInvoke.DestroyIcon(shinfo.hIcon);

        largeImageList.Add(shinfo.iIcon, imageSource);

        return imageSource;
    }

    public static unsafe string GetDisplayName(string path)
    {
        uint result = 0;
        if (PInvoke.SHParseDisplayName(path, null, out var ppidl, 0, &result) == 0)
        {
            ITEMIDLIST idlChild = new ITEMIDLIST();
            idlChild.mkid.cb = (ushort)sizeof(SHITEMID);

            ITEMIDLIST* pidlChild = &idlChild;
            ITEMIDLIST** ppidlChild = &pidlChild;

            try
            {
                fixed (Guid* riid = &IID_IShellFolder)
                    if (PInvoke.SHBindToParent(ppidl, riid, out object shellFolder, ppidlChild) == 0)
                    {
                        IShellFolder psf = (IShellFolder)shellFolder;

                        try
                        {
                            psf.GetDisplayNameOf(**ppidlChild, SHGDNF.SHGDN_INFOLDER, out STRRET pName);

                            char[] c = new char[1024];
                            fixed (char* pc = &c[0])
                            {
                                PInvoke.StrRetToBuf(ref pName, **ppidlChild, new PWSTR(pc), (uint)c.Length);
                                return new string(pc);
                            }
                        }
                        finally
                        {
                            Marshal.ReleaseComObject(psf);
                        }
                    }
                    else
                    {
                        throw new Exception("SHBindToParent failed.");
                    }
            }
            finally
            {
                Marshal.FreeCoTaskMem((nint)ppidl);
            }
        }
        else
        {
            throw new Exception("SHParseDisplayName failed.");
        }
    }

    public static unsafe void OpenContextMenu(string path)
    {
        using ContextMenuWndProcHelper wndHelper = new();

        GetUiObjectOfFile(path, IID_IContextMenu, new HWND(wndHelper.Hwnd), out IContextMenu pcm);

        try
        {
            HMENU hmenu = PInvoke.CreatePopupMenu();
            if (hmenu == 0)
                throw new Exception("CreatePopupMenu failed");

            pcm.QueryContextMenu(hmenu, 0, SCRATCH_QCM_FIRST, SCRATCH_QCM_LAST, PInvoke.CMF_NORMAL);

            wndHelper.ContextMenu2 = pcm as My.IContextMenu2;
            wndHelper.ContextMenu3 = pcm as My.IContextMenu3;

            PInvoke.GetCursorPos(out System.Drawing.Point cursorPosition);

            PInvoke.SetForegroundWindow(new HWND(wndHelper.Hwnd));

            int iCmd = PInvoke.TrackPopupMenuEx(hmenu, (uint)TRACK_POPUP_MENU_FLAGS.TPM_RETURNCMD, cursorPosition.X, cursorPosition.Y, new HWND(wndHelper.Hwnd), null);

            if (iCmd > 0)
            {
                CMINVOKECOMMANDINFOEX info = new()
                {
                    cbSize = (uint)sizeof(CMINVOKECOMMANDINFOEX),
                    fMask = PInvoke.SEE_MASK_UNICODE | CMIC_MASK_PTINVOKE,
                    hwnd = new HWND(wndHelper.Hwnd),
                    lpVerb = new PCSTR((byte*)(iCmd - SCRATCH_QCM_FIRST)),
                    lpVerbW = new PCWSTR((char*)(iCmd - SCRATCH_QCM_FIRST)),
                    nShow = (int)SHOW_WINDOW_CMD.SW_SHOWNORMAL,
                    ptInvoke = cursorPosition
                };

                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                    info.fMask |= CMIC_MASK_CONTROL_DOWN;
                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                    info.fMask |= CMIC_MASK_SHIFT_DOWN;

                try
                {
                    pcm.InvokeCommand((CMINVOKECOMMANDINFO*)&info);
                }
                catch
                {
                    // Do nothing. This happens for example if a user cancels an UAC prompt
                }
            }

            PInvoke.DestroyMenu(hmenu);
        }
        finally
        {
            wndHelper.ContextMenu3 = null;
            wndHelper.ContextMenu2 = null;
            Marshal.ReleaseComObject(pcm);
        }
    }

    private unsafe static void GetUiObjectOfFile<T>(string path, Guid interfaceGuid, HWND hwnd, [NotNull] out T? uiObject)
    {
        uiObject = default;
        uint result = 0;
        if (PInvoke.SHParseDisplayName(path, null, out var ppidl, 0, &result) == 0)
        {
            ITEMIDLIST idlChild = new ITEMIDLIST();
            idlChild.mkid.cb = (ushort)sizeof(SHITEMID);

            ITEMIDLIST* pidlChild = &idlChild;
            ITEMIDLIST** ppidlChild = &pidlChild;

            try
            {
                fixed (Guid* riid = &IID_IShellFolder)
                    if (PInvoke.SHBindToParent(ppidl, riid, out object shellFolder, ppidlChild) == 0)
                    {
                        IShellFolder psf = (IShellFolder)shellFolder;

                        uint rgfReserved = 0;
                        try
                        {
                            psf.GetUIObjectOf(hwnd, 1, ppidlChild, &interfaceGuid, &rgfReserved, out object ppv);
                            uiObject = (T)ppv;
                        }
                        finally
                        {
                            Marshal.ReleaseComObject(psf);
                        }
                    }
                    else
                    {
                        throw new Exception("SHBindToParent failed.");
                    }
            }
            finally
            {
                Marshal.FreeCoTaskMem((nint)ppidl);
            }
        }
        else
        {
            throw new Exception("SHParseDisplayName failed.");
        }
    }
}
