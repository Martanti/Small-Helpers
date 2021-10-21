export module Array2D;

export import <memory>;

/**
 * @brief A class used for 2D array creation.
 * Internally uses a 1D array for reducing cache misses
 * @tparam T Class that will be the value of the array.
*/
export template <class T>
class Array2D
{
private:
	// In order to avoid coupling this to a GLM - custom u int made private so it won't interfere with GLM one
	typedef unsigned int uint;
	uint const mHeight;
	uint const mWidth;
	std::unique_ptr<T[]> mArray;
private:
	inline uint const& index(uint const& x, uint const& y) const
	{
		return (x + y * mWidth);
	}
public:

	/**
	 * @brief Create a 2D array.
	 * @param width Desired width of the array.
	 * @param height Desired height of the array.
	*/
	Array2D(uint const& width, uint const& height) : mWidth(width), mHeight(height), mArray(nullptr)
	{
		mArray = std::make_unique<T[]>(mWidth * mHeight);
	}

	/**
	 * @brief Get item in provided position.
	 * @param widthPos X in 2D.
	 * @param heightPos Y in 2D.
	 * @return Value located at X Y. Not intended for modifying.
	*/
	inline T const& GetAt(uint const& widthPos, uint const& heightPos) const
	{
		return mArray[index(widthPos, heightPos)];
	}

	/**
	 * @brief Set the value located at X Y.
	 * @param widthPos X in 2D.
	 * @param heightPos Y in 2D.
	 * @param value Desired value that will be set.
	*/
	inline void SetAt(uint const& widthPos, uint const& heightPos, T const& value)
	{
		mArray[index(widthPos, heightPos)] = value;
	}

	/**
	 * @brief Get height of the array.
	 * @return Height. Not intended for modifying.
	*/
	inline uint const& GetHeight() const
	{
		return mHeight;
	}

	/**
	 * @brief Get the width of the array.
	 * @return Width. Not intended for modifying.
	*/
	inline uint const& GetWidth() const
	{
		return mWidth;
	}
};