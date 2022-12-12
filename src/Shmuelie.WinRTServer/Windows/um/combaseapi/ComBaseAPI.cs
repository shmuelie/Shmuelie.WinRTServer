using System;
using System.Runtime.InteropServices;

namespace Shmuelie.Interop.Windows;

/// <summary>
/// APIs from the <c>combaseapi</c> header.
/// </summary>
internal static unsafe class ComBaseAPI
{
    /// <summary>
    /// Creates and default-initializes a single object of the class associated with a specified CLSID.
    /// </summary>
    /// <param name="rclsid">The CLSID associated with the data and code that will be used to create the object.</param>
    /// <param name="pUnkOuter">If <see langword="null"/>, indicates that the object is not being created as part of an aggregate. If non-<see langword="null"/>, pointer to the aggregate object's <see cref="IUnknown" /> interface (the controlling <see cref="IUnknown" />).</param>
    /// <param name="dwClsContext">Context in which the code that manages the newly created object will run. The values are taken from the enumeration <see cref="CLSCTX" />.</param>
    /// <param name="riid">A reference to the identifier of the interface to be used to communicate with the object.</param>
    /// <param name="ppv">Address of pointer variable that receives the interface pointer requested in <paramref name="riid"/>. Upon successful return, *<paramref name="ppv"/> contains the requested interface pointer. Upon failure, *<paramref name="ppv"/> contains <see langword="null"/>.</param>
    /// <returns>
    /// <para>This function can return the following values.</para>
    /// <list type="table">
    ///   <listheader>
    ///     <description>Return code</description>
    ///     <description>Description</description>
    ///   </listheader>
    ///   <item>
    ///     <description><c>S_OK</c></description>
    ///     <description>An instance of the specified object class was successfully created.</description>
    ///   </item>
    ///   <item>
    ///     <description><c>REGDB_E_CLASSNOTREG</c></description>
    ///     <description>A specified class is not registered in the registration database. Also can indicate that the type of server you requested in the <see cref="CLSCTX" /> enumeration is not registered or the values for the server types in the registry are corrupt.</description>
    ///   </item>
    ///   <item>
    ///     <description><c>CLASS_E_NOAGGREGATION</c></description>
    ///     <description>This class cannot be created as part of an aggregate.</description>
    ///   </item>
    ///   <item>
    ///     <description><c>E_NOINTERFACE</c></description>
    ///     <description>The specified class does not implement the requested interface, or the controlling <see cref="IUnknown" /> does not expose the requested interface.</description>
    ///   </item>
    ///   <item>
    ///     <description><c>E_POINTER</c></description>
    ///     <description>The <paramref name="ppv"/> parameter is <see langword="null"/>.</description>
    ///   </item>
    /// </list>
    /// </returns>
    [DllImport("ole32", ExactSpelling = true)]
    public static extern int CoCreateInstance(Guid* rclsid, IUnknown* pUnkOuter, uint dwClsContext, Guid* riid, void** ppv);

    /// <summary>
    /// Registers an EXE class object with OLE so other applications can connect to it.
    /// </summary>
    /// <param name="rclsid">The CLSID to be registered.</param>
    /// <param name="pUnk">A pointer to the <see cref="IUnknown" /> interface on the class object whose availability is being published.</param>
    /// <param name="dwClsContext">The context in which the executable code is to be run. For information on these context values, see the <see cref="CLSCTX" /> enumeration.</param>
    /// <param name="flags">Indicates how connections are made to the class object. For information on these flags, see the <see cref="REGCLS" /> enumeration.</param>
    /// <param name="lpdwRegister">A pointer to a value that identifies the class object registered; later used by the <see cref="CoRevokeClassObject" /> function to revoke the registration.</param>
    /// <returns>
    /// <para>This function can return the standard return values E_INVALIDARG, E_OUTOFMEMORY, and E_UNEXPECTED, as well as the following values.</para>
    /// <list type="table">
    ///   <listheader>
    ///     <description>Return code</description>
    ///     <description>Description</description>
    ///   </listheader>
    ///   <item>
    ///     <description><c>S_OK</c></description>
    ///     <description>The class object was registered successfully.</description>
    ///   </item>
    /// </list>
    /// </returns>
    [DllImport("ole32", ExactSpelling = true)]
    public static extern int CoRegisterClassObject(Guid* rclsid, IUnknown* pUnk, uint dwClsContext, uint flags, uint* lpdwRegister);

    /// <summary>
    /// Called by a server that can register multiple class objects to inform the SCM about all registered classes, and permits activation requests for those class objects.
    /// </summary>
    /// <returns>This function returns S_OK to indicate that the activation of class objects was successfully resumed.</returns>
    [DllImport("ole32", ExactSpelling = true)]
    public static extern int CoResumeClassObjects();

    /// <summary>
    /// Prevents any new activation requests from the SCM on all class objects registered within the process.
    /// </summary>
    /// <returns>This function returns S_OK to indicate that the activation of class objects was successfully suspended.</returns>
    [DllImport("ole32", ExactSpelling = true)]
    public static extern int CoSuspendClassObjects();

    /// <summary>
    /// Increments a global per-process reference count.
    /// </summary>
    /// <returns>The current reference count.</returns>
    [DllImport("ole32", ExactSpelling = true)]
    public static extern uint CoAddRefServerProcess();

    /// <summary>
    /// Decrements the global per-process reference count.
    /// </summary>
    /// <returns>If the server application should initiate its cleanup, the function returns 0; otherwise, the function returns a nonzero value.</returns>
    [DllImport("ole32", ExactSpelling = true)]
    public static extern uint CoReleaseServerProcess();

    /// <summary>
    /// Informs OLE that a class object, previously registered with the <see cref="CoRegisterClassObject" /> function, is no longer available for use.
    /// </summary>
    /// <param name="dwRegister">A token previously returned from the <see cref="CoRegisterClassObject" /> function.</param>
    /// <returns>
    /// <para>This function can return the standard return values E_INVALIDARG, E_OUTOFMEMORY, and E_UNEXPECTED, as well as the following values.</para>
    /// <list type="table">
    ///   <listheader>
    ///     <description>Return code</description>
    ///     <description>Description</description>
    ///   </listheader>
    ///   <item>
    ///     <description><c>S_OK</c></description>
    ///     <description>The class object was revoked successfully.</description>
    ///   </item>
    /// </list>
    /// </returns>
    [DllImport("ole32", ExactSpelling = true)]
    public static extern int CoRevokeClassObject(uint dwRegister);
}