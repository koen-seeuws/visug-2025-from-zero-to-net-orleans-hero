window.scrollHelper = {
    scrollToBottom: function (containerId) {
        const container = document.getElementById(containerId);

        if(!container)
            return;
        
        container.scrollTop = container.scrollHeight;
        
    },
    
    scrollToBottomIfPreviouslyNearBottom: function (containerId, margin = 25) {
        const container = document.getElementById(containerId);
        
        if(!container)
            return;
        
        if(container.children.length <= 2) {
            requestAnimationFrame(() => {
                container.scrollTop = container.scrollHeight;
            });
        }

        const secondToLastChild = container.children[container.children.length - 2]
        const distanceFromBottom = container.scrollHeight - (container.scrollTop + container.clientHeight + secondToLastChild.offsetHeight);
        const wasNearBottom = distanceFromBottom <= margin;

        if (!wasNearBottom) {
            // do nothing; user wants to read older messages
            return;
        }

        requestAnimationFrame(() => {
            container.scrollTop = container.scrollHeight;
        });
    }
};