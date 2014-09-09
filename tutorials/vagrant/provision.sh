apt-get update -y > /dev/null

# Install prerequisites
apt-get install \
	git \
	cmake \
	build-essential \
	nasm \
	php5-cli \
	libsdl-dev \
	libogg-dev \
	-y

apt-get upgrade -y > /dev/null
