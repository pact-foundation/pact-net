# Introduction

["Pact"](https://github.com/realestate-com-au/pact) is an implementation of "consumer driven contract" testing that allows mocking of responses in the consumer codebase, and verification of the interactions in the provider codebase. The initial implementation was written in Ruby for Rack apps, however a consumer and provider may be implemented in different programming languages, so the "mocking" and the "verifying" steps would be best supported by libraries in their respective project's native languages. Given that the pact file is written in JSON, it should be straightforward to implement a pact library in any language, however, to get the best experience and most reliability of out mixing pact libraries, the matching logic for the requests and responses needs to be identical. There is little confidence to be gained in having your pacts "pass" if the logic used to verify a "pass" is inconsistent between implementations.

To support consistency of matching logic, this specification has been developed as a benchmark that all pact libraries can check themselves against if they want to ensure consistency with other pact libraries.

### Pact Specificaton Philosophy

* Be as strict as we reasonably can with what we send out (requests). We should know and be able to control exactly what a consumer is sending out. Information should not be allowed to "leak" silently.
* Be as loose as we reasonably can with what we accept (responses). A provider should be able to send extra information that this particular consumer does not care about, without breaking this consumer.
* When writing the matching rules, err on the side of being more strict now, because it will break fewer things to be looser later, than to get stricter later.

Note: One implications of this philosophy is that you cannot verify, using pact, that a key or a header will _not_ be present in a response. You can only verify what _is_.

### Version 1.0.0

#### Request matching

##### Request method

Exact string match, case sensitive (expects lower case). Open to the idea that this could be case insensitive.

##### Request path

Exact string match, case sensitive, as paths are generally case sensitive. Trailing slashes should not be ignored, as they could potentially have significance.

##### Request headers

Exact string match for expected header names and values. Allow unexpected headers to be sent out, as frameworks and network utilities are likely to set their own headers (eg. User-Agent), and it would increase the maintenance burden to have to track all of those.

##### Request body

* Do not allow unexpected keys to be sent in the body. See "Pact Specificaton Philosophy" in main README.
* Do not allow unexpected items in an array. Most parsing code will do a "for each" on an array, and if we expect one item, but two go out, we have "leaked" information.

#### Response matching

##### Response status

Exact integer match.

##### Response headers

Exact string match for expected header names and values. Allow unexpected headers to be sent back, as in reality, as extra headers will be added by network utilities and server frameworks.

##### Response body

* Allow unexpected keys to be sent back in the body. See "Pact Specificaton Philosophy" in main README.
* Do not allow unexpected items in an array. Most parsing code will do a "for each" on an array, and if we expect one item, but receive two, we might not have exercised the correct code to handle that second item in our consumer tests.
