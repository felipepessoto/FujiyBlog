﻿fujiyBlog = {
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
                    })
                }
            });
        }
    }
}

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