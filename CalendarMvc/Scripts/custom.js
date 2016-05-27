$(function () {
    $("#scheduler").kendoScheduler({
        date: new Date("2013/6/13"),
        startTime: new Date("2013/6/13 07:00 AM"),
        height: 600,
        views: [
            "day",
            { type: "workWeek", selected: true },
            "week",
            "month",
            "agenda",
            { type: "timeline", eventHeight: 50 }
        ],
        timezone: "Etc/UTC",
        dataSource: {
            batch: true,
            transport: {
                read: {
                    url: "//localhost:27592/Scheduler/Read",
                    dataType: "jsonp"
                },
                update: {
                    url: "//demos.telerik.com/kendo-ui/service/tasks/update",
                    dataType: "jsonp"
                },
                create: {
                    url: "//demos.telerik.com/kendo-ui/service/tasks/create",
                    dataType: "jsonp"
                },
                destroy: {
                    url: "//demos.telerik.com/kendo-ui/service/tasks/destroy",
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
                        taskId: { from: "TaskID", type: "number" },
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
            },
            filter: {
                logic: "or",
                filters: [
                    { field: "ownerId", operator: "eq", value: 1 },
                    { field: "ownerId", operator: "eq", value: 2 }
                ]
            }
        },
        resources: [
            {
                field: "ownerId",
                title: "Owner",
                dataSource: [
                    { text: "Alex", value: 1, color: "#f8a398" },
                    { text: "Bob", value: 2, color: "#51a0ed" },
                    { text: "Charlie", value: 3, color: "#56ca85" }
                ]
            }
        ]
    });

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