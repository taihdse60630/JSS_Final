$(function () {
    var spinner = $( ".spinner" ).spinner();

    //BEGIN JQUERY TABLE SORTER
    $(".tablesorter").tablesorter({
        headers: {
            0: {
                sorter: false
            }
        }
    });
    //END JQUERY TABLE SORTER

    //BEGIN JQUERY DATE PICKER
    $('.datepicker-filter').datepicker({
        autoclose: true
    });
    //END JQUERY DATE PICKER

    $('.submit-action').click(function(e) {
        if($('.table-group-action-select').val() > 0){
            $('.tb-alert-success').fadeIn();
            $('.tb-alert-error').fadeOut();
        } else{
            $('.tb-alert-success').fadeOut();
            $('.tb-alert-error').fadeIn();
        }
    });

    //BEGIN PAGING TABLE
    pager(0);

    $(".gw-prev").click(function(e){
        if(!$(this).hasClass('disabled')){
            pager(-1);
            load();
        }
    });
    $(".gw-next").click(function(e){
        if(!$(this).hasClass('disabled')){
            pager(1);
            load();
        }
    });
    $(".gw-pageSize").change(function(e){
        load();
    });
    //END PAGING TABLE

});

function pager(p){
    var page = Math.max(1, (parseInt($(".gw-page").val()) + p));
    $(".gw-page").val(page);

    if(1 == page){
        $(".gw-prev").addClass('disabled');
    }
    else{
        $(".gw-prev").removeClass('disabled');
    }

    if(10 == page){
        $(".gw-next").addClass('disabled');
    }
    else{
        $(".gw-next").removeClass('disabled');
    }
}

function load(){
    var status = [
        '<span class="label label-sm label-success">Approved</span>',
        '<span class="label label-sm label-info">Pending</span>',
        '<span class="label label-sm label-warning">Suspended</span>',
        '<span class="label label-sm label-danger">Blocked</span>'
    ];

    var skill = [
        '<div class="progress progress-xs mbs" title="75%" data-hover="tooltip"><div class="progress-bar progress-bar-green" style="width: 75%;" aria-valuemax="100" aria-valuemin="0" aria-valuenow="75" role="progressbar"><span class="sr-only">75% Complete</span></div></div>',
        '<div class="progress progress-xs mbs" title="40%" data-hover="tooltip"><div class="progress-bar progress-bar-red six-sec-ease-in-out" style="width: 40%;" aria-valuemax="100" aria-valuemin="0" aria-valuenow="40" role="progressbar"><span class="sr-only">40% Complete</span></div></div>',
        '<div class="progress progress-xs mbs" title="80%" data-hover="tooltip"><div class="progress-bar progress-bar-yellow" style="width: 80%;" aria-valuemax="100" aria-valuemin="0" aria-valuenow="80" role="progressbar"><span class="sr-only">80% Complete</span></div></div>',
        '<div class="progress progress-xs mbs" title="60%" data-hover="tooltip"><div class="progress-bar progress-bar-violet" style="width: 60%;" aria-valuemax="100" aria-valuemin="0" aria-valuenow="60" role="progressbar"><span class="sr-only">60% Complete</span></div></div>'
    ];

    $(".grid-view tbody > tr").remove();

    var pageSize = parseInt($(".gw-pageSize").val());
    var page = parseInt($(".gw-page").val());
    var s = (page - 1) * pageSize;

    var html = $(".gw-row").val();
    var result = '';
    var m = s + pageSize;
    for(s; s<m; s++){
        var r = Math.floor((Math.random() * 3) + 1);
        var k = Math.floor((Math.random() * 3) + 1);
        result += html.replace("{index}", s).replace("{skill}", skill[k]).replace("{status}", status[r]);
    }

    $(".grid-view tbody").html(result);
    var spinner = $( ".spinner" ).spinner();
}
