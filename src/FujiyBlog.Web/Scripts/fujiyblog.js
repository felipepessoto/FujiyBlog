fujiyBlog = {
    searchForm: {
        sendSearch: function (terms) {
            window.location.href = fujiyBlogUrls.Search_Index + '/?terms=' + terms;
        }
    },
    widget: {

        currentDialog: null,

        showEditWidget: function (result) {
            if (fujiyBlog.widget.currentDialog) {
                fujiyBlog.widget.currentDialog.remove();
            }

            fujiyBlog.widget.currentDialog = $('<div>' + result + '</div>').dialog({ height: 500,
                width: 750,
                modal: true,
                overlay: { opacity: 0.7, background: 'black' }
            });
        },

        saveWidgetSettings: function (id, jsonSettings) {
            $.ajax({
                url: fujiyBlogUrls.Widget_Edit,
                type: 'post',
                data: { widgetSettingId: id, settings: jsonSettings }
            }).success(fujiyBlog.widget.refreshWidget);
        },

        refreshWidget: function (htmlContent) {
            var newContext = $(htmlContent);
            var elementToReplace = $('#' + newContext.attr('id'));

            newContext.replaceAll(elementToReplace);

            $('#' + newContext.attr('id')).effect('pulsate');

            if (fujiyBlog.widget.currentDialog) {
                fujiyBlog.widget.currentDialog.dialog('close');
            }
        },

        showAddedWidget: function (idZoneElement, result) {
            if (result) {
                $('#' + idZoneElement).append(result);
            }
        },

        startDragAndDrop: function () {
            $('.widgetzone').sortable({ placeholder: 'ui-state-highlight', forcePlaceholderSize: true, handle: '.move',
                update: function (event, ui) {
                    var widgetsOrder = $(this).sortable('toArray').toString();
                    $.ajax({
                        url: fujiyBlogUrls.Widget_Sort,
                        type: 'post',
                        data: { widgetsOrder: widgetsOrder }
                    });
                }
            });
        }
    },
    comments: {
        addCommentSuccess: function (response) {

            if (response.errorMessage) {
                alert(response.errorMessage);
            }
            else {
                var parentCommentId = $('#ParentCommentId').val();

                if (parentCommentId) {
                    $('#replies_' + parentCommentId.toString()).append(response);
                } else {
                    $('#commentlist').append(response);
                }
            }
            if (window.Recaptcha) {
                window.Recaptcha.reload();
            }
        },
        replyComment: function (parentCommentId) {
            //Reset and remove padding left, to have enought space to write a comment
            if (!fujiyBlog.comments.originalRepliesPaddingLeft) {
                fujiyBlog.comments.originalRepliesPaddingLeft = $('.comment-replies:first').css('padding-left');
            }
            fujiyBlog.comments.resetRepliesPaddingLeft();
            
            var parentsWrappers = $('#replies_' + parentCommentId).parents('.comment-replies');
            if (parentsWrappers.length > 1) {
                parentsWrappers.animate({ 'padding-left': '0px' }, 'fast', 'linear');
            }
            //Reset and remove padding left, to have enought space to write a comment
            
            $('#ParentCommentId').val(parentCommentId);
            $('#cancel-replying').show();
            $('#add-comment-form').hide().prependTo($('#replies_' + parentCommentId)).slideDown(400, 'linear');

        },
        cancelReplyComment: function () {

            fujiyBlog.comments.resetRepliesPaddingLeft();

            $('#ParentCommentId').val('');
            $('#cancel-replying').hide();
            $('#add-comment-form').insertAfter($('#commentlist'));
        },
        resetRepliesPaddingLeft: function () {
            $('.comment-replies').animate({ 'padding-left': fujiyBlog.comments.originalRepliesPaddingLeft }, 'fast', 'linear');
        },
        originalRepliesPaddingLeft: null
    }
};

$(function () {
    $('.searchWidgetTerms').keypress(function (e) {
        if (e.which == 13) {
            $(this).next().click();
        }
    });

    $('.datetime-picker').datetimepicker({
        ampm: true
    });

    if (window.shouldStartDragAndDrop) {
        $(function () { fujiyBlog.widget.startDragAndDrop(); });
    }
});