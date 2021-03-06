import React from 'react';
import PropTypes from 'prop-types';
import { connect } from 'react-redux';
import wordlistApiActions from '../../actions/wordPairsApiActions';
import { AppBarContext } from '../TopAppBar/AppBarContext';
import usePrevious from '../../hooks/usePrevious';
import useCheckboxList from '../../hooks/useCheckboxList';
import WordPairsUncheckButton from './WordPairsUncheckButton';
import WordPairsDeleteButton from './WordPairsDeleteButton';
import Wordlist from './WordlistPage';

function WordlistPageContainer({ wordPairs, pullWordPairs, deleteWordPairs }) {
    React.useEffect(() => {
        if (!wordPairs.length) {
            pullWordPairs();
        }
    }, []);

    const { checked, handleToggle, handleReset } = useCheckboxList();

    React.useEffect(() => {
        handleReset();
    }, [wordPairs]);

    const { customizeAppBar, resetAppBar } = React.useContext(AppBarContext);

    const prevChecked = usePrevious(checked);

    React.useEffect(() => {
        if (checked.length) {
            if (!prevChecked.length) {
                resetAppBar({
                    title: `Выбрано: ${checked.length}`,
                    color: 'secondary',
                    leftElement: <WordPairsUncheckButton handleUncheck={() => handleReset([])} />,
                    rightElements: [<WordPairsDeleteButton handleWordPairsDeletion={() => deleteWordPairs(checked)} />]
                });
            } else {
                customizeAppBar({
                    title: `Выбрано: ${checked.length}`,
                    rightElements: [<WordPairsDeleteButton handleWordPairsDeletion={() => deleteWordPairs(checked)} />]
                });
            }
        } else {
            resetAppBar({
                title: 'Словарь'
            });
        }
    }, [checked]);

    return (
        <Wordlist
            wordPairs={[...wordPairs].sort((firstWordPair, secondWordPair) =>
                firstWordPair.wordForeign.localeCompare(secondWordPair.wordForeign)
            )}
            checked={checked}
            handleToggle={handleToggle}
        />
    );
}

WordlistPageContainer.propTypes = {
    wordPairs: PropTypes.arrayOf(
        PropTypes.shape({
            wordForeign: PropTypes.string.isRequired,
            wordNative: PropTypes.string.isRequired
        }).isRequired
    ).isRequired,
    pullWordPairs: PropTypes.func.isRequired,
    deleteWordPairs: PropTypes.func.isRequired
};

const mapStateToProps = store => {
    return {
        wordPairs: store.wordPairs
    };
};

const mapDispatchToProps = dispatch => {
    return {
        pullWordPairs: () => dispatch(wordlistApiActions.pullWordPairs()),
        deleteWordPairs: pairIds => dispatch(wordlistApiActions.deleteWordPairs(pairIds))
    };
};

export default connect(
    mapStateToProps,
    mapDispatchToProps
)(WordlistPageContainer);
