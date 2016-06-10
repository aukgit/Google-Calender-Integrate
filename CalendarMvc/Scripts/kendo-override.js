var kendoSchedulerOverride = function ($kendoScheduler, eventOwners) {
    /// <summary>
    /// modify kendo scheduler to our custom markup.
    /// </summary>
    /// <param name="$kendoScheduler" type="type"></param>
    console.log($kendoScheduler);
    var $timeline = $kendoScheduler.find(".k-scheduler-timelineview");

    this.isEmpty = function(v) {
        return v === undefined || v === null || v === "" || v.length === 0;
    }

    if ($timeline.length > 0 && !$timeline.hasClass("bound")) {
        var $secondRow = $timeline.find("tr:nth-child(2)");
        var $nodes = $secondRow.find("td");
        var $leftNode = $($nodes[0]);
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
                    $cells.removeClass("active");
                    $this.addClass("active");
                    alert($this.text());
                });
            }
           
            var addColumnsleftNodes = function ($cell) {
                var $parent = $cell.parent();
                var $html = $($parent.html());

                var ownerName = $html.text();
                var owner = getOwnerByName(ownerName);
                if (!isEmpty(owner)) {
                    $parent.find("th").attr('id', owner.id);
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


        var $cells = $leftNode.find("th");
        processLeftNode($cells);
    }
}