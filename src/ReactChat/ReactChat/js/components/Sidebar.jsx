var Sidebar = React.createClass({
    getInitialState: function() {
        return { hideExamples: false };
    },
    toggleExamples: function(e) {
        this.setState({ hideExamples: !this.state.hideExamples });
    },
	isWinforms: function() {
		return window.nativeHost.platform === 'winforms';
	},
    render: function() {
        var height = this.state.hideExamples ? '25px' : 'auto',
            label = this.state.hideExamples ? 'show' : 'hide';
        return (
            <div id="right">
                <div id="users">
                {this.props.users.map(function(user) { 
                    return <User key={user.userId} user={user} />;
                })}
                </div>
                <div id="examples" style={{ height: height }}>
                    <span style={{position: 'absolute', top: '2px', right: '7px'}} onClick={this.toggleExamples}>{label}</span>
                    <span data-click="sendCommand">
                        <h4><a href="#">Example Commands</a></h4>
                        { this.isWinforms() ? <div>/cmd.toggleFormBorder</div> : null}
                        <div>/cmd.announce This is your captain speaking ...</div>
                        <div>/cmd.toggle$#channels</div>
                        <h4><a href="#">CSS</a></h4>
                        <div>/css.background-image url(http://bit.ly/1yIJOBH)</div>
                        <div>/css.background-image url(/img/disco.jpg)</div>
                        <div>@me /css.background$#top #673ab7</div>
                        <div>/css.background$#bottom #0091ea</div>
                        <div>/css.background$#right #fffde7</div>
                        <div>/css.color$#welcome #ff0</div>
                        <div>/css.visibility$img,a hidden</div>
                        <div>/css.visibility$img,a visible</div>
                        <h4><a href="#">Receivers</a></h4>
                        <div>/tv.watch https://youtu.be/9bZkp7q19f0</div>
                        <div>/tv.watch https://servicestack.net/img/logo-220.png</div>
                        <div>@me /tv.off</div>
                        { this.isWinforms() ? <div>/windows.shrink</div> : null}
                        { this.isWinforms() ? <div>/windows.grow</div> : null}
                        { this.isWinforms() ? <div>/windows.dance</div> : null}
                        <div>/window.location http://google.com</div>
                        <h4><a href="#">Triggers</a></h4>
                        <div>/trigger.customEvent arg</div>
                    </span>
                </div>
            </div>
        );
    }
});