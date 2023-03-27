#include "pch.h"
#include "Test.h"
#include <stdio.h>

Test::Test(int x) {
	this->x = x+5;
}

int Test::add(int amount)
{
	this->x += amount;
	return this->x;
}

int Test::subtract(int amount)
{
	this->x -= amount;
	return this->x;
}

int Test::getX() {
	return this->x;
}

int Test::setX(int value)
{
	this->x = value;
	return this->x;
}

void Test(const char* arr)
{
	printf("char-arr: %s\n", arr);
}