$(function () {
    var controller = "/Scheduler/";
    var initializeScheduler = function (eventOwners) {
        var $scheduler = $("#scheduler");

        $scheduler.kendoScheduler({
            date: new Date("2013/6/13"),
            startTime: new Date("2013/6/13 07:00 AM"),
            eventHeight: 50,
            majorTick: 60,
            height: 1024,
            views: [
                "day",
                "workWeek",
                "week",
                { type: "month", selected: true },
                "agenda",
                { type: "timeline", eventHeight: 50 }
            ],
            timezone: "Etc/UTC",
            dataSource: {
                batch: true,
                transport: {
                    read: {
                        url: "//demos.telerik.com/kendo-ui/service/meetings",
                        dataType: "jsonp"
                    },
                    update: {
                        url: "//demos.telerik.com/kendo-ui/service/meetings/update",
                        dataType: "jsonp"
                    },
                    create: {
                        url: "//demos.telerik.com/kendo-ui/service/meetings/create",
                        dataType: "jsonp"
                    },
                    destroy: {
                        url: "//demos.telerik.com/kendo-ui/service/meetings/destroy",
                        dataType: "jsonp"
                    },
                    parameterMap: function (options, operation) {
                        if (operation !== "read" && options.models) {
                            return { models: kendo.stringify(options.models) };
                        }
                    }
                },
                schema: {
                    model: {
                        id: "taskId",
                        fields: {
                            taskId: { from: "TaskID" },
                            title: { from: "Title", defaultValue: "No title", validation: { required: true } },
                            start: { type: "date", from: "Start" },
                            end: { type: "date", from: "End" },
                            startTimezone: { from: "StartTimezone" },
                            endTimezone: { from: "EndTimezone" },
                            description: { from: "Description" },
                            recurrenceId: { from: "RecurrenceID" },
                            recurrenceRule: { from: "RecurrenceRule" },
                            recurrenceException: { from: "RecurrenceException" },
                            ownerId: { from: "OwnerID", defaultValue: 1 },
                            isAllDay: { type: "boolean", from: "IsAllDay" }
                        }
                    }
                }
                //filter: {
                //    logic: "or",
                //    filters: [
                //        { field: "ownerId", operator: "eq", value: 1 },
                //        { field: "ownerId", operator: "eq", value: 2 }
                //    ]
                //}
            },
            group: {
                resources: ["Attendees"],
                orientation: "vertical"
            },
            resources: [
                {
                    field: "attendees",
                    name: "Attendees",
                    dataSource: [
                        { text: "Alex", value: 1, color: "#f8a398" },
                        { text: "Bob", value: 2, color: "#51a0ed" },
                        { text: "Charlie", value: 3, color: "#56ca85" }
                    ],
                    multiple: true,
                    title: "Attendees"
                }
            ]
        });
    }

    var url = controller + "GetOwners";
    var isEmpty = function (variable) {
        return variable === undefined || variable === null || variable.length === 0 || variable === "";
    }
    var isInTestingMode = true;
    jQuery.ajax({
        method: "GET", // by default "GET"
        url: url,
        dataType: "JSON" //, // "Text" , "HTML", "xml", "script" 

    }).done(function (owners) {
        var $eventOwnerHtml = $("#event-owners");
        for (var i = 0; i < owners.length; i++) {
            var owner = owners[i];
            if (isEmpty(owner.color)) {
                owners[i].color = "#51a0ed";
            }
            var $li = $("<li></li>", {
                'text': owner.text,
                'style': 'float:left;background-color:' + owner.color,
            });
            $eventOwnerHtml.append($li);
        }
        initializeScheduler(owners);
    }).fail(function (jqXHR, textStatus, exceptionMessage) {
        console.log("Request failed: " + exceptionMessage);
    }).always(function () {
        console.log("complete");
    });
    var $colorPicker = $(".color-picker");
    if ($colorPicker.length > 0) {
        var preview = function (e) {
            $("#background").css("background-color", e.value);
        }

        $colorPicker.kendoColorPicker({
            value: $colorPicker.val(),
            buttons: false,
            select: preview
        });
  
    }
    var datePickerComponentEnable = function () {
        var $dateTimePicker = $(".datetimepicker-start");
        if ($dateTimePicker.length > 0) {
            $dateTimePicker.datetimepicker({
                pickDate: true, //en/disables the date picker
                pickTime: true, //en/disables the time picker
                useMinutes: true, //en/disables the minutes picker
                useSeconds: true, //en/disables the seconds picker
                useCurrent: true, //when true, picker will set the value to the current date/time     
                minuteStepping: 1, //set the minute stepping
                defaultDate: "", //sets a default date, accepts js dates, strings and moment objects
                disabledDates: [], //an array of dates that cannot be selected
                enabledDates: [], //an array of dates that can be selected
                sideBySide: true //show the date and time picker side by side
            });
        }
        var $datePicker = $(".datepicker-start");
        if ($datePicker.length > 0) {
            $datePicker.datetimepicker({
                pickDate: true, //en/disables the date picker
                pickTime: false, //en/disables the time picker
                useMinutes: false, //en/disables the minutes picker
                useSeconds: false, //en/disables the seconds picker
                useCurrent: true, //when true, picker will set the value to the current date/time     
                minuteStepping: 1, //set the minute stepping
                defaultDate: "", //sets a default date, accepts js dates, strings and moment objects
                disabledDates: [], //an array of dates that cannot be selected
                enabledDates: [], //an array of dates that can be selected
                sideBySide: true //show the date and time picker side by side
            });
        }
    };
    datePickerComponentEnable();

    if ($.detectBrowser.isInternetExplorer) {
        var $html = $("#html-start");
        $html.addClass("ie");
    }
});