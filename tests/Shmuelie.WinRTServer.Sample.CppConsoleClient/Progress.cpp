#include "pch.h"
#include "Progress.h"

#include <iostream>

void IndefiniteSpinner::ShowSpinner()
{
    if (!m_spinnerJob.valid() && !m_spinnerRunning && !m_canceled)
    {
        m_spinnerRunning = true;
        m_spinnerJob = std::async(std::launch::async, &IndefiniteSpinner::ShowSpinnerInternal, this);
    }
}

void IndefiniteSpinner::StopSpinner()
{
    if (!m_canceled && m_spinnerJob.valid() && m_spinnerRunning)
    {
        m_canceled = true;
        m_spinnerJob.get();
    }
}

void IndefiniteSpinner::ShowSpinnerInternal()
{
    char spinnerChars[] = { '-', '\\', '|', '/' };

    // First wait for a small amount of time to enable a fast task to skip
    // showing anything, or a progress task to skip straight to progress.
    Sleep(100);

    if (!m_canceled)
    {
        // Indent two spaces for the spinner, but three here so that we can overwrite it in the loop.
        std::cout << "   ";

        for (size_t i = 0; !m_canceled; ++i)
        {
            std::cout << '\b' << spinnerChars[i % ARRAYSIZE(spinnerChars)] << std::flush;
            Sleep(250);
        }

        std::cout << "\b \r";
    }

    m_canceled = false;
    m_spinnerRunning = false;
}

void ProgressBar::ShowProgress(uint64_t current, uint64_t maximum)
{
    if (current < m_lastCurrent)
    {
        ClearLine();
    }

    ShowProgressNoVT(current, maximum);

    m_lastCurrent = current;
    m_isVisible = true;
}

void ProgressBar::EndProgress(bool hideProgressWhenDone)
{
    if (m_isVisible)
    {
        if (hideProgressWhenDone)
        {
            ClearLine();
        }
        else
        {
            std::cout << std::endl;
        }

        m_isVisible = false;
    }
}

void ProgressBar::ClearLine()
{
    // Best effort when no VT (arbitrary number of spaces that seems to work)
    std::cout << "\r";
}

void ProgressBar::ShowProgressNoVT(uint64_t current, uint64_t maximum)
{
    std::cout << "\r  ";

    if (maximum)
    {
        const char* const blockOn = u8"\x2588";
        const char* const blockOff = u8"\x2592";
        constexpr size_t blockWidth = 30;

        double percentage = static_cast<double>(current) / maximum;
        size_t blocksOn = static_cast<size_t>(std::floor(percentage * blockWidth));

        for (size_t i = 0; i < blocksOn; ++i)
        {
            std::cout << blockOn;
        }

        for (size_t i = 0; i < blockWidth - blocksOn; ++i)
        {
            std::cout << blockOff;
        }

        std::cout << "  ";

        std::cout << static_cast<int>(percentage * 100) << '%';
    }
    else
    {
        std::cout << current << '%';
    }
}