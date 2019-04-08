import React from 'react';
import PropTypes from 'prop-types';
import withStyles from '@material-ui/core/styles/withStyles';
import Card from '@material-ui/core/Card';
import CardActionArea from '@material-ui/core/CardActionArea';
import CardActions from '@material-ui/core/CardActions';
import CardContent from '@material-ui/core/CardContent';
import CardMedia from '@material-ui/core/CardMedia';
import Typography from '@material-ui/core/Typography';
import ProfileEditingContainer from '../../containers/Profile/ProfileEditingContainer';

const styles = theme => ({
    card: {
        maxWidth: 240,
        marginLeft: 'auto',
        marginRight: 'auto',
        marginTop: theme.spacing.unit * 2,
    },
    media: {
        height: 0,
        paddingTop: '100%',
    },
});

function Profile({ editingAvailable, userInfo, classes }) {
    const { avatarPath, nickName, experience } = userInfo;

    return (
        <Card className={classes.card}>
            <CardActionArea>
                {avatarPath && (
                    <CardMedia
                        className={classes.media}
                        image={avatarPath}
                        title="Avatar"
                    />)}
                <CardContent>
                    <Typography gutterBottom variant="h5">
                        {nickName}
                    </Typography>
                    <Typography component="p">
                        {experience} опыта
                    </Typography>
                </CardContent>
            </CardActionArea>
            <CardActions>
                {editingAvailable && <ProfileEditingContainer userInfo={userInfo} />}
            </CardActions>
        </Card>
    );
}

Profile.propTypes = {
    editingAvailable: PropTypes.bool.isRequired,
    userInfo: PropTypes.shape({
        userId: PropTypes.number,
        nickName: PropTypes.string.isRequired,
        avatarPath: PropTypes.string,
        experience: PropTypes.number.isRequired
    }).isRequired,
    classes: PropTypes.object.isRequired
};

export default withStyles(styles)(Profile);