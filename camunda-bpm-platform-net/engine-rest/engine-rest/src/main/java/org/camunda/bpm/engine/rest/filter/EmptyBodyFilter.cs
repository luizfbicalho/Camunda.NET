using System.IO;

/*
 * Copyright Camunda Services GmbH and/or licensed to Camunda Services GmbH
 * under one or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information regarding copyright
 * ownership. Camunda licenses this file to you under the Apache License,
 * Version 2.0; you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace org.camunda.bpm.engine.rest.filter
{

	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	public class EmptyBodyFilter : Filter
	{

	  protected internal static readonly Pattern CONTENT_TYPE_JSON_PATTERN = Pattern.compile("^application\\/json((;)(.*)?)?$", Pattern.CASE_INSENSITIVE);

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void doFilter(final ServletRequest req, final ServletResponse resp, FilterChain chain) throws java.io.IOException, ServletException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public override void doFilter(ServletRequest req, ServletResponse resp, FilterChain chain)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean isContentTypeJson = CONTENT_TYPE_JSON_PATTERN.matcher(req.getContentType() == null ? "" : req.getContentType()).find();
		bool isContentTypeJson = CONTENT_TYPE_JSON_PATTERN.matcher(req.ContentType == null ? "" : req.ContentType).find();

		if (isContentTypeJson)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.PushbackInputStream requestBody = new java.io.PushbackInputStream(req.getInputStream());
		  PushbackInputStream requestBody = new PushbackInputStream(req.InputStream);
		  int firstByte = requestBody.read();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean isBodyEmpty = firstByte == -1;
		  bool isBodyEmpty = firstByte == -1;
		  requestBody.unread(firstByte);

		  HttpServletRequestWrapper wrappedRequest = new HttpServletRequestWrapperAnonymousInnerClass(this, (HttpServletRequest) req, requestBody, isBodyEmpty);

		  chain.doFilter(wrappedRequest, resp);
		}
		else
		{
		  chain.doFilter(req, resp);
		}
	  }

	  private class HttpServletRequestWrapperAnonymousInnerClass : HttpServletRequestWrapper
	  {
		  private readonly EmptyBodyFilter outerInstance;

		  private PushbackInputStream requestBody;
		  private bool isBodyEmpty;

		  public HttpServletRequestWrapperAnonymousInnerClass(EmptyBodyFilter outerInstance, HttpServletRequest HttpServletRequest) req, PushbackInputStream requestBody, bool isBodyEmpty) : base(HttpServletRequest) req)
		  {
			  this.outerInstance = outerInstance;
			  this.requestBody = requestBody;
			  this.isBodyEmpty = isBodyEmpty;
		  }


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public ServletInputStream getInputStream() throws java.io.IOException
		  public override ServletInputStream InputStream
		  {
			  get
			  {
    
				return new ServletInputStreamAnonymousInnerClass(this);
			  }
		  }

		  private class ServletInputStreamAnonymousInnerClass : ServletInputStream
		  {
			  private readonly HttpServletRequestWrapperAnonymousInnerClass outerInstance;

			  public ServletInputStreamAnonymousInnerClass(HttpServletRequestWrapperAnonymousInnerClass outerInstance)
			  {
				  this.outerInstance = outerInstance;
				  inputStream = outerInstance.isBodyEmpty ? new MemoryStream("{}".GetBytes(Charset.forName("UTF-8"))) : outerInstance.requestBody;
			  }


			  internal Stream inputStream;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public int read() throws java.io.IOException
			  public override int read()
			  {
				return inputStream.read();
			  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public int available() throws java.io.IOException
			  public override int available()
			  {
				return inputStream.available();
			  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void close() throws java.io.IOException
			  public override void close()
			  {
				inputStream.close();
			  }

			  public override void mark(int readlimit)
			  {
				  lock (this)
				  {
					inputStream.mark(readlimit);
				  }
			  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public synchronized void reset() throws java.io.IOException
			  public override void reset()
			  {
				  lock (this)
				  {
					inputStream.reset();
				  }
			  }

			  public override bool markSupported()
			  {
				return inputStream.markSupported();
			  }

		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public java.io.BufferedReader getReader() throws java.io.IOException
		  public override StreamReader Reader
		  {
			  get
			  {
				return new StreamReader(this.InputStream);
			  }
		  }

	  }

	  public override void destroy()
	  {

	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void init(FilterConfig filterConfig) throws ServletException
	  public override void init(FilterConfig filterConfig)
	  {

	  }

	}

}