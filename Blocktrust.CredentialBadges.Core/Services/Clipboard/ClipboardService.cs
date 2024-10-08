﻿using Microsoft.JSInterop;

namespace Blocktrust.CredentialBadges.Core.Services.Clipboard;

public class ClipboardService
{
    private IJSRuntime JSRuntime { get; set; }

    public ClipboardService(IJSRuntime jsRuntime)
    {
        JSRuntime = jsRuntime;
    }

    public async Task CopyTextToClipboard(string? text)
    {
        if (!string.IsNullOrEmpty(text))
        {
            await JSRuntime.InvokeVoidAsync("clipboardCopy.copyText", text);
        }
    }
}