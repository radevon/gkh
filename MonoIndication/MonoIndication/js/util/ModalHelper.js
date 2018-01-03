function createModal(heading, bodyContent, footerContent)
    {
        var html =  '<div id="modalWindow" class="modal fade" tabindex="-1" role="dialog" aria-labelledby="confirm-modal" aria-hidden="true">';
        html += '<div class="modal-dialog">';
        html += '<div class="modal-content">';
        html += '<div class="modal-header">';
        html += '<a class="close" data-dismiss="modal">×</a>';
        html += '<h4 class="text-primary">' + heading + '</h4>';
        html += '</div>';
        html += '<div class="modal-body">';
        html += bodyContent;
        html += '</div>';
        html += '<div class="modal-footer">';
        html += footerContent;
        html += '</div>';  // footer
        html += '</div>';  // content
        html += '</div>';  // dialog
        html += '</div>';  // modalWindow
    
        $('body').append(html);
        
        $('#modalWindow').on('hide.bs.modal', function (e) {
            $(this).remove();
            $('.modal-backdrop').remove();
        });
       

        $("#modalWindow").modal('show');

      
}