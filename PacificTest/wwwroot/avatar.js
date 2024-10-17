'use strict';


async function getAvatarUrl(userId) {
    const defaultUrl = 'https://api.dicebear.com/8.x/pixel-art/png?seed=default&size=150';

    try {
        const endpoint = `http://localhost:7126/avatar?userIdentifier=${userId}`;
        if (!endpoint)
            return defaultUrl;

        console.log(`fetching ${endpoint}`)
        const response = await fetch(endpoint);
        const responseBody = await response.json();
        console.log(responseBody);
        return responseBody.url;
    }
    catch (e) {
        console.error(e);
        return defaultUrl;
    }
}

