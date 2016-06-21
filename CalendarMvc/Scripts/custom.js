/// <reference path="kendo-override.js" />
Date.prototype.yyyymmdd = function () {
    var yyyy = this.getFullYear().toString();
    var mm = (this.getMonth() + 1).toString(); // getMonth() is zero-based
    var dd = this.getDate().toString();
    return yyyy + (mm[1] ? mm : "0" + mm[0]) + (dd[1] ? dd : "0" + dd[0]); // padding
};
$(function () {
    var controller = "/Scheduler/";
    var kendoControl;
    var initializeScheduler = function (eventOwners) {
        var $scheduler = $("#scheduler");
        

        kendoControl = $scheduler.kendoScheduler({
            height: 600,
            views: [
                { type: "month" },
                { type: "timeline", selected: true, eventHeight: 16 }
            ],
            date: kendo.date.today(),
            //editable: false,
            //timezone: "Etc/UTC",
            minorTickCount:4,
            eventTemplate: $("#event-template").html(),
            allDaySlot: false,
            footer: false,
            dataSource: {
                batch: true,
                transport: {
                    read: {
                        url: controller + "Read",
                        dataType: "json"
                    },
                    //update: {
                    //    //url: "//demos.telerik.com/kendo-ui/service/tasks/update",
                    //    url: controller + "update",
                    //    dataType: "json"
                    //},
                    //create: {
                    //    url: controller + "create",
                    //    dataType: "json"
                    //},
                    //destroy: {
                    //    url: controller + "destroy",
                    //    dataType: "json"
                    //},
                    parameterMap: function (options, operation) {
                        if (operation !== "read" && options.models) {
                            var result = options.models[0];
                            result.Start = result.Start.toUTCString();
                            result.End = result.End.toUTCString();

                            return options.models[0];
                        } else if (operation === "read") {
                            // read
                        }

                    }
                },
               
                change: function (e) {
                   // var start = e.start; //selection start date
                   // var end = e.end; //selection end date
                   // var slots = e.slots; //list of selected slots
                   // var events = e.items; //list of selected Scheduler events

                   // var message = "change:: selection from {0:g} till {1:g}";

                   // if (events.length) {
                   //     message += ". The selected event is '" + events[events.length - 1].title + "'";
                   // }

                   //console.log(message + ", start : " +  start + ", end :" + end);
                    kendoSchedulerOverride($scheduler, eventOwners);
                   console.log("change event");
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
                            isAllDay: { type: "boolean", from: "IsAllDay" },
                            color: { from: "Color" },
                            borderClass: { from: "BorderClass" },

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
                resources: ["ownerId"],
                orientation: "vertical"
            },
            resources: [
                {
                    field: "ownerId",
                    title: "Account",
                    dataSource: eventOwners,
                    //multiple: true
                    //    [
                    //    { text: "Alex", value: 1, color: "#f8a398" },
                    //    { text: "Bob", value: 2, color: "#51a0ed" },
                    //    { text: "Charlie", value: 3, color: "#56ca85" }
                    //]
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
        //var $eventOwnerHtml = $("#event-owners");
        //for (var i = 0; i < owners.length; i++) {
        //    var owner = owners[i];
        //    if (isEmpty(owner.color)) {
        //        owners[i].color = "#51a0ed";
        //    }
        //    var $li = $("<li></li>", {
        //        'text': owner.text,
        //        'style': 'float:left;background-color:' + owner.color,
        //    });
        //    $eventOwnerHtml.append($li);
        //}
        initializeScheduler(owners);
    }).fail(function (jqXHR, textStatus, exceptionMessage) {
        console.log("Request failed: " + exceptionMessage);
    }).always(function () {
        //console.log("complete");
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