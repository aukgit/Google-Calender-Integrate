var kendoSchedulerOverride = function ($kendoScheduler, eventOwners) {
    /// <summary>
    /// modify kendo scheduler to our custom markup.
    /// </summary>
    /// <param name="$kendoScheduler" type="type"></param>
    console.log($kendoScheduler);
    var $timeline = $kendoScheduler.find(".k-scheduler-timelineview");

    var getFormattedDate = function (d) { // create prototype for Date
        var month = (d.getMonth() + 1);
        if (d.getMonth() < 10) {
            month = "0" + month;
        }
        return d.getFullYear() + "-" + month + "-" + d.getDate();
    };

    this.isEmpty = function (v) {
        return v === undefined || v === null || v === "" || v.length === 0;
    }

    if ($timeline.length > 0 && !$timeline.hasClass("bound")) {
        var $secondRow = $timeline.find("tr:nth-child(2)");
        var $nodes = $secondRow.find("td");
        var $leftNode = $($nodes[0]);
        var $leftNodeTable = $leftNode.find(".k-scheduler-table");
        var $rightNode = $($nodes[1]);
        console.log($leftNode);
        console.log($rightNode);
        $timeline.addClass("bound");
        var getOwnerByName = function (name) {
            for (var i = 0; i < eventOwners.length; i++) {
                var owner = eventOwners[i];
                if (owner.text === name) {
                    return owner;
                }
            }
            return null;
        }
        console.log(eventOwners);
        var processLeftNode = function ($cells) {
            var bindLeftNodeCells = function ($cells) {
                $cells.click(function (e) {
                    var $this = $(this);
                    $cells.removeClass("active"); //remove color from clinician name
                    $cells.siblings().removeClass('active');// remove color from timezone
                    $this.addClass("active");//add color to clinician name
                    $this.siblings().addClass("active");// add color to timezone
                    $("#notification-date").val(getFormattedDate(new Date()));
                    $("#schedule-date").val(getFormattedDate(new Date()));
                    $("#subject-visit-comment").val(e.target.innerText);
                    $(".k-scheduler-content .k-scheduler-table tr").removeClass("active");
                    $(".k-scheduler-content .k-scheduler-table tr:eq(" + $cells.index($this) + ")").addClass("active");
                });
            }

            var addColumnsleftNodes = function ($cell) {
                var $parent = $cell.parent();
                var $html = $($parent.html());

                var ownerName = $html.text();
                var owner = getOwnerByName(ownerName);
                if (!isEmpty(owner)) {
                    $parent.find("th").attr('id', "kendo-owner-id-" + owner.id);
                    $parent.find("th").attr('data-id', owner.id);
                    $parent.find("th").addClass("kendo-owner-item");
                    $html.text(owner.time + " " + owner.timezone);
                }
                $parent.append($html);
            }

            bindLeftNodeCells($cells);

            for (var i = 0; i < $cells.length; i++) {
                var $cell = $($cells[i]);
                addColumnsleftNodes($cell);
            }
        }

        var $cells = $leftNodeTable.find("th");
        $leftNodeTable.attr("id", "kendo-owners-table");
        processLeftNode($cells);
        kendoBindEvents($kendoScheduler);
    }
}

var kendoBindEvents = function ($kendoScheduler) {
    var $form = $("#example-form");
    var i;
    $form.submit(function (e) {
        e.preventDefault();
        // evnt id insert in the hidden
        var $ownerIDs = $(".kendo-owner-item");
        for (i = 1; i <= $ownerIDs.length; i++) {
            if ($ownerIDs.filter("[data-id=" + i + "]").hasClass("active")) {
                $("#owner-id-hidden").val(i);
                //$.ajax({
                //    url: $form.attr("action"),
                //    data: $form.serializeArray(),
                //    success: function (response) {
                //        //$kendoScheduler.data("kendoScheduler").add(response);
                //        var binder = $("#scheduler").data("kendoScheduler"),
                //            list = binder._data;
                //        var lastItem = list[list.length - 1];
                //        var cloned = $.extend({}, response, lastItem);
                //        cloned.start = response.Start;
                //        cloned.end = response.End;
                //        cloned.title = response.Title;
                //        cloned.color = response.Color;
                //        cloned.taskId = response.TaskId;
                //        //cloned.id = response.taskId;
                //        cloned.borderClass = response.BorderClass;
                //        cloned.description = response.Description;
                //        binder.push(cloned);
                //        binder.refresh();
                //    }
                //});
                
                
                    //var location = window.location;
                    //var hostName = location.host;
                    ////var path = location.pathname;
                    //var url = "http://" + hostName + "/Scheduler";
                    
                
            
                this.submit();

                return;
            }
        }
    });
}