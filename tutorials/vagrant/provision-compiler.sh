# Install prerequisites for compilation.
apt-get install \
	build-essential \
	nasm \
	libsdl-dev \
	libogg-dev \
	libxft-dev \
	libxxf86vm-dev \
	-y

# Install CMake manually since the archive versions are old.
if ! hash cmake; then
	wget http://www.cmake.org/files/v2.8/cmake-2.8.12.2.tar.gz
	tar -xzvf cmake-2.8.12.2.tar.gz
	rm cmake-2.8.12.2.tar.gz
	cd cmake-2.8.12.2
	./bootstrap && make && make install
	cd ..
	rm -rf cmake-2.8.12.2
fi
