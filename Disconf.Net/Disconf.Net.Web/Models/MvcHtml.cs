using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Disconf.Net.Web.Models
{
    public static class MvcHtml
    {
        public static MvcHtmlString GetUploadHtml()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<script type='text/template' id='qq-template'><div class='qq-uploader-selector qq-uploader'>");
            sb.Append("<div class='qq-upload-drop-area-selector qq-upload-drop-area' qq-hide-dropzone style='display:none;'>");
            sb.Append("<span>Drop files here to upload</span></div><div class='qq-upload-button-selector qq-upload-button' style='height:28px;'>");
            sb.Append("<div>&nbsp;</div></div><span class='qq-drop-processing-selector qq-drop-processing' style='display:none;'>");
            sb.Append("<span>Processing dropped files...</span>");
            sb.Append("<span class='qq-drop-processing-spinner-selector qq-drop-processing-spinner'></span></span>");
            sb.Append("<ul class='qq-upload-list-selector qq-upload-list' style='display:none;'><li><div class='qq-progress-bar-container-selector'>");
            sb.Append("<div class='qq-progress-bar-selector qq-progress-bar'></div></div>");
            sb.Append("<span class='qq-upload-spinner-selector qq-upload-spinner'></span><span class='qq-edit-filename-icon-selector qq-edit-filename-icon'></span>");
            sb.Append("<span class='qq-upload-file-selector qq-upload-file'></span><input class='qq-edit-filename-selector qq-edit-filename' tabindex='0' type='text'>");
            sb.Append("<span class='qq-upload-size-selector qq-upload-size'></span><a class='qq-upload-cancel-selector qq-upload-cancel' href='#'>Cancel</a>");
            sb.Append("<a class='qq-upload-retry-selector qq-upload-retry' href='#'>Retry</a><a class='qq-upload-delete-selector qq-upload-delete' href='#'>Delete</a>");
            sb.Append("<span class='qq-upload-status-text-selector qq-upload-status-text'></span></li></ul></div></script>");
            return MvcHtmlString.Create(sb.ToString());
        }
    }
}