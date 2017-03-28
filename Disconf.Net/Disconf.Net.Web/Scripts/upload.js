var configType = false;
$(function () {
    $('.flash_area').fineUploader({
        request: {
            endpoint: '/Template/FileUpload',
            params: {
            }
        },
        messages: {
            sizeError: "上传文件不能超出5M."
        },
        multiple: false,
        validation: {
            sizeLimit: 5242880
        },
        showMessage: function (message) {
            alert(message);
        }
    }).on("complete", function (event, id, name, responseJSON, xhr) {
        if (!responseJSON.IsSuccess) {
            alert("上传失败，处理过程中发生错误！")
        } else {
            if (configType) {
                vue.value = responseJSON.Data;
            } else {
                vue.defaultValue = responseJSON.Data;
            }
        }
    });
});