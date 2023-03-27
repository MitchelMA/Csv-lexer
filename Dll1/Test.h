class Test {
	int x;
public:
	Test(int x);
	int add(int amount);
	int subtract(int amount);
	int getX();
	int setX(int value);
};

extern "C" {
	__declspec(dllexport)
	void* Create(int x) {
		return new Test(x);
	}

	__declspec(dllexport)
	int Add(Test* t, int amount)
	{
		return t->add(amount);
	}

	__declspec(dllexport)
	int Subtract(Test* t, int amount)
	{
		return t->subtract(amount);
	}

	__declspec(dllexport)
	int GetX(Test* t)
	{
		return t->getX();
	}

	__declspec(dllexport)
	int SetX(Test* t, int value)
	{
		return t->setX(value);
	}

	__declspec(dllexport)
	void Test(const char *arr);
}