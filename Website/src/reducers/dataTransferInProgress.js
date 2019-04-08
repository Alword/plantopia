import dataTransferConstants from '../constants/dataTransferConstants';

const dataTransferInProgress = (state = false, action) => {
    switch (action.type) {
        case dataTransferConstants.DATA_TRANSFER_BEGIN:
            return true;
        case dataTransferConstants.DATA_TRANSFER_END:
            return false;
        default:
            return state;
    }
};

export default dataTransferInProgress;